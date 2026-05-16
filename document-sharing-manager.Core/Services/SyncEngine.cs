using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using document_sharing_manager.Core.Data;
using document_sharing_manager.Core.DTOs;
using document_sharing_manager.Core.Domain;
using document_sharing_manager.Core.Interfaces;
using document_sharing_manager.Core.Common;
using Newtonsoft.Json;

namespace document_sharing_manager.Core.Services
{
    public enum SyncType { Upload, Download, Delete }

    public class SyncTask
    {
        public int DocumentId { get; set; }
        public SyncType Type { get; set; }
        public int ServerId { get; set; }
    }

    public class SyncEngine : ISyncService, IDisposable
    {
        public event EventHandler? SyncCompleted;
        private readonly IDocumentRepository _repository;
        
        // Static HttpClient to prevent socket exhaustion
        private static readonly HttpClient _httpClient = new();
        
        static SyncEngine()
        {
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            // Set a reasonable timeout for large file uploads
            _httpClient.Timeout = TimeSpan.FromMinutes(10);
        }

        private readonly ConcurrentQueue<SyncTask> _taskQueue = new();
        // Track enqueued document IDs to avoid O(N) lookup in queue
        private readonly ConcurrentDictionary<string, byte> _enqueuedTasks = new();
        
        private readonly SemaphoreSlim _signal = new(0);
        private readonly System.Collections.Concurrent.ConcurrentDictionary<int, ManagedServer> _servers;
        private readonly CancellationTokenSource _cts = new();
        private bool _disposed = false;
        private readonly SynchronizationContext _syncContext = SynchronizationContext.Current;
        private readonly System.Threading.Timer _debounceTimer;
        private readonly ConcurrentDictionary<int, SemaphoreSlim> _serverLocks = new();
        private bool _isRunning = false;

        public SyncEngine(IDocumentRepository repository)
        {
            _repository = repository;
            
            // Load servers from DB
            var activeServers = DatabaseHelper.GetManagedServers().Where(s => s.IsActive);
            _servers = new System.Collections.Concurrent.ConcurrentDictionary<int, ManagedServer>(
                activeServers.ToDictionary(s => s.Id, s => s));

            _debounceTimer = new System.Threading.Timer(_ => TriggerSyncCompleted(), null, Timeout.Infinite, Timeout.Infinite);
        }

        private void TriggerSyncCompleted()
        {
            if (_syncContext != null)
                _syncContext.Post(_ => SyncCompleted?.Invoke(this, EventArgs.Empty), null);
            else
                SyncCompleted?.Invoke(this, EventArgs.Empty);
        }

        private void RequestSyncRefresh()
        {
            _debounceTimer.Change(500, Timeout.Infinite); // Debounce for 500ms
        }

        public void AddServer(ManagedServer server)
        {
            if (_servers.TryAdd(server.Id, server))
            {
                // Trigger immediate sync for this new server
                _ = SyncServerAsync(server, _cts.Token);
            }
        }

        public void RemoveServer(int serverId)
        {
            if (_servers.TryRemove(serverId, out _) && _serverLocks.TryRemove(serverId, out var sem))
            {
                sem.Dispose();
            }
        }

        public void Start()
        {
            if (_isRunning) return;
            _isRunning = true;
            Task.Run(ProcessQueueAsync, _cts.Token);
            
            // Background loop for periodic server polling (Auto-refresh from servers)
            Task.Run(async () => {
                while (!_cts.Token.IsCancellationRequested)
                {
                    try { await SyncAsync(_cts.Token); }
                    catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Periodic Sync Error: {ex.Message}"); }
                    await Task.Delay(TimeSpan.FromMinutes(5), _cts.Token);
                }
            }, _cts.Token);
        }

        public void Stop()
        {
            if (_disposed) return;
            try
            {
                if (!_cts.IsCancellationRequested) _cts.Cancel();
            }
            catch (ObjectDisposedException) { }
            _isRunning = false;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            Stop();
            foreach (var sem in _serverLocks.Values) sem.Dispose();
            _serverLocks.Clear();
            _debounceTimer?.Dispose();
            try { _cts.Dispose(); } catch (ObjectDisposedException) { }
            _signal.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task<Result> SyncAsync(CancellationToken ct = default)
        {
            var tasks = _servers.Values.Select(s => SyncServerAsync(s, ct));
            await Task.WhenAll(tasks);
            return Result.Success();
        }

        public async Task<Result> SyncServerAsync(ManagedServer server, CancellationToken ct = default)
        {
            var serverLock = _serverLocks.GetOrAdd(server.Id, _ => new SemaphoreSlim(1, 1));
            
            // Try to acquire the lock, if already syncing, just skip to avoid queueing up multiple syncs
            if (!await serverLock.WaitAsync(0, ct))
            {
                return Result.Success(); // Already syncing this server
            }

            try
            {
                // 1. Pull from this specific server
                await PullFromServerAsync(server, ct);

                // 2. Claim documents with no server assigned yet and enqueue uploads/deletes
                var documents = await _repository.GetPendingSyncDocumentsAsync(UserSession.CurrentUserId, ct);
                foreach (var doc in documents.Where(d => d.SyncStatus != 0 && (d.ServerId == server.Id || d.ServerId == null)))
                {
                    if (doc.ServerId == null)
                    {
                        doc.ServerId = server.Id;
                        await _repository.UpdateAsync(doc, ct); // Persist the server assignment
                    }
                    
                    // Re-check after update
                    if (doc.ServerId == server.Id)
                    {
                        if (doc.IsDeleted)
                        {
                            Enqueue(doc, SyncType.Delete, server.Id);
                        }
                        else
                        {
                            Enqueue(doc, SyncType.Upload, server.Id);
                        }
                    }
                }

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }
            finally
            {
                serverLock.Release();
            }
        }

        private async Task<HttpResponseMessage> SendAuthenticatedRequestAsync(ManagedServer server, HttpMethod method, string relativeUrl, HttpContent? content = null, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken ct = default)
        {
            var authClient = server.GetAuthClient();
            
            // Helper to create the request
            HttpRequestMessage CreateRequest()
            {
                var req = new HttpRequestMessage(method, $"{server.BaseUrl.TrimEnd('/')}/{relativeUrl.TrimStart('/')}");
                if (!string.IsNullOrEmpty(server.AccessToken))
                {
                    req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", server.AccessToken);
                }
                if (content != null && method != HttpMethod.Get)
                {
                    // Note: content cannot be reused across requests if it's a stream. 
                    // This is handled in the calling methods.
                    req.Content = content; 
                }
                return req;
            }

            var response = await _httpClient.SendAsync(CreateRequest(), completionOption, ct);

            // If 401, try to refresh once
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized && !string.IsNullOrEmpty(server.RefreshToken))
            {
                System.Diagnostics.Debug.WriteLine($"Token expired for {server.Name}, attempting refresh...");
                bool refreshed = await authClient.RefreshTokensAsync(server.RefreshToken);
                if (refreshed)
                {
                    // Update the server object and DB
                    server.AccessToken = UserSession.AccessToken!;
                    server.RefreshToken = UserSession.RefreshToken!;
                    DatabaseHelper.UpdateServerTokens(server.Id, server.AccessToken, server.RefreshToken);
                    
                    // Retry request - Note: caller must ensure content is recreatable if it was a stream
                    // For GET requests, it's always safe. For POST with stream, caller will handle it.
                    if (method == HttpMethod.Get)
                    {
                        return await _httpClient.SendAsync(CreateRequest(), completionOption, ct);
                    }
                }
            }

            return response;
        }

        public async Task<Result> PullFromServerAsync(ManagedServer server, CancellationToken ct = default)
        {
            try
            {
                var response = await SendAuthenticatedRequestAsync(server, HttpMethod.Get, "api/documents", null, HttpCompletionOption.ResponseContentRead, ct);
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var serverDocs = JsonConvert.DeserializeObject<List<Document>>(json);
                    if (serverDocs == null) return Result.Failure("Invalid server response");

                    // Get current local documents for this server to detect remote deletions
                    var localDocsForServer = (await _repository.GetAllByUserIdAsync(UserSession.CurrentUserId, ct))
                        .Where(d => d.ServerId == server.Id && !d.IsDeleted)
                        .ToList();

                    var serverRemoteIds = new HashSet<Guid>(serverDocs.Select(d => d.RemoteId));

                    foreach (var serverDoc in serverDocs)
                    {
                        var localDoc = await _repository.GetByRemoteIdAsync(serverDoc.RemoteId, ct);
                        if (localDoc == null)
                        {
                            // Add new document from server
                            serverDoc.UserId = UserSession.CurrentUserId;
                            serverDoc.ServerId = server.Id;
                            serverDoc.SyncStatus = 2; // PendingDownload - needs to download actual file content

                            // Map DuongDan to local pattern - Include RemoteId to prevent collisions
                            string safeFileName = string.Join("_", serverDoc.Ten.Split(Path.GetInvalidFileNameChars()));
                            serverDoc.DuongDan = Path.Combine(FileStorageService.DefaultFolder, server.Id.ToString(), $"{serverDoc.RemoteId.ToString().Substring(0, 8)}_{safeFileName}");
                            
                            await _repository.AddAsync(serverDoc, ct);
                            // Enqueue(serverDoc, SyncType.Download, server.Id); // Removed: Only download on demand
                        }
                        else if (serverDoc.Version > localDoc.Version)
                        {
                            // Newer version on server
                            localDoc.Version = serverDoc.Version;
                            localDoc.Ten = serverDoc.Ten;
                            localDoc.DinhDang = serverDoc.DinhDang;
                            localDoc.GhiChu = serverDoc.GhiChu;
                            localDoc.SyncStatus = 2; // PendingDownload
                            localDoc.IsDeleted = false; // Restore if it was soft-deleted locally but updated on server
                            localDoc.ServerId = server.Id;
                            
                            await _repository.UpdateAsync(localDoc, ct);
                            // Enqueue(localDoc, SyncType.Download, server.Id); // Removed: Only download on demand
                        }
                    }

                    // Detect remote deletions: if a local doc for this server is not in serverDocs, it was deleted on server
                    // Safety: Only delete if it's already been synced (Version > 0) or is not pending upload
                    foreach (var localDoc in localDocsForServer)
                    {
                        if (!serverRemoteIds.Contains(localDoc.RemoteId) && (localDoc.Version > 0 || localDoc.SyncStatus == 0))
                        {
                            // Delete local file too
                            string fullPath = FileStorageService.ResolvePath(localDoc.DuongDan);
                            if (File.Exists(fullPath)) File.Delete(fullPath);
                            
                            await _repository.DeleteAsync(localDoc.Id, ct);
                        }
                    }

                    if (serverDocs.Count > 0 || localDocsForServer.Any(ld => !serverRemoteIds.Contains(ld.RemoteId)))
                    {
                        RequestSyncRefresh();
                    }
                    return Result.Success();
                }
                return Result.Failure($"Server returned {response.StatusCode}");
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }

        public async Task<Result<IEnumerable<DocumentDto>>> GetPendingUploadsAsync(CancellationToken ct = default)
        {
             var docs = await _repository.GetPendingSyncDocumentsAsync(UserSession.CurrentUserId, ct);
             var dtos = docs.Where(d => d.SyncStatus == 1 || d.SyncStatus == 3)
                            .Select(d => new DocumentDto { Id = d.Id, FileName = d.Ten });
             return Result<IEnumerable<DocumentDto>>.Success(dtos);
        }

        public async Task<Result<IEnumerable<DocumentDto>>> GetPendingDownloadsAsync(CancellationToken ct = default)
        {
             var docs = await _repository.GetPendingSyncDocumentsAsync(UserSession.CurrentUserId, ct);
             var dtos = docs.Where(d => d.SyncStatus == 2).Select(d => new DocumentDto { Id = d.Id, FileName = d.Ten });
             return Result<IEnumerable<DocumentDto>>.Success(dtos);
        }

        public void Enqueue(Document doc, SyncType type, int serverId)
        {
            string key = $"{doc.Id}_{type}_{serverId}";
            if (_enqueuedTasks.TryAdd(key, 0))
            {
                _taskQueue.Enqueue(new SyncTask { DocumentId = doc.Id, Type = type, ServerId = serverId });
                _signal.Release();
            }
        }

        private async Task ProcessQueueAsync()
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                try
                {
                    await _signal.WaitAsync(_cts.Token);
                    
                    if (_taskQueue.TryDequeue(out var task))
                    {
                        string taskKey = $"{task.DocumentId}_{task.Type}_{task.ServerId}";
                        
                        if (!_servers.TryGetValue(task.ServerId, out var server))
                        {
                            _enqueuedTasks.TryRemove(taskKey, out _);
                            continue;
                        }

                        // Load fresh document state from repository
                        var doc = await _repository.GetByIdAsync(task.DocumentId, _cts.Token);
                        if (doc == null)
                        {
                            _enqueuedTasks.TryRemove(taskKey, out _);
                            continue;
                        }

                        try
                        {
                            if (task.Type == SyncType.Upload)
                                await HandleUploadAsync(doc, server, _cts.Token);
                            else if (task.Type == SyncType.Download)
                                await HandleDownloadAsync(doc, server, _cts.Token);
                            else if (task.Type == SyncType.Delete)
                                await HandleRemoteDeleteAsync(doc, server, _cts.Token);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Task processing error: {ex.Message}");
                        }
                        finally
                        {
                            _enqueuedTasks.TryRemove(taskKey, out _);
                            // Trigger UI refresh debounced
                            RequestSyncRefresh();
                        }
                    }
                }
                catch (OperationCanceledException) { break; }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"SyncEngine Worker Error: {ex.Message}");
                    await Task.Delay(5000, _cts.Token);
                }
            }
        }


        private async Task HandleUploadAsync(Document doc, ManagedServer server, CancellationToken ct)
        {
            try
            {
                string fullPath = FileStorageService.ResolvePath(doc.DuongDan);
                if (!File.Exists(fullPath)) return;

                async Task<HttpResponseMessage> SendUploadRequest()
                {
                    // Re-create content for each attempt because stream will be consumed
                    var multipartContent = new MultipartFormDataContent
                    {
                        { new StringContent(doc.RemoteId.ToString()), "remoteId" },
                        { new StringContent(doc.Version.ToString()), "localVersion" },
                        { new StringContent(server.Id.ToString()), "serverId" }
                    };
                    if (!string.IsNullOrEmpty(doc.Ten)) multipartContent.Add(new StringContent(doc.Ten), "ten");
                    if (doc.GhiChu != null) multipartContent.Add(new StringContent(doc.GhiChu), "ghiChu");

                    var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 4096, useAsync: true);
                    var fileContent = new StreamContent(fileStream);
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    multipartContent.Add(fileContent, "file", Path.GetFileName(fullPath));

                    return await SendAuthenticatedRequestAsync(server, HttpMethod.Post, "api/documents/sync-stream", multipartContent, HttpCompletionOption.ResponseContentRead, ct);
                }

                var response = await SendUploadRequest();
                
                if (response.IsSuccessStatusCode)
                {
                    var resultJson = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<SyncResponse>(resultJson);
                    
                    if (result?.Success == true)
                    {
                        int oldVersion = doc.Version;
                        int newVersion = result.CurrentVersion;

                        // 1. Update DB synchronously on background thread to avoid race condition
                        await _repository.UpdateSyncStatusAsync(doc.Id, 0, newVersion, oldVersion, newVersion, ct);

                        // 2. Update object on UI thread via Post for UI thread safety
                        void UpdateLocal()
                        {
                            doc.Version = newVersion;
                            doc.SyncStatus = 0; // 0: Synced
                            doc.LocalVersion = newVersion; 
                        }

                        if (_syncContext != null)
                        {
                            _syncContext.Send(_ => UpdateLocal(), null);
                        }
                        else
                        {
                            UpdateLocal();
                        }
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    await HandleConflictAsync(doc, ct);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Upload failed for {doc.Ten}: {ex.Message}");
            }
        }

        private async Task HandleRemoteDeleteAsync(Document doc, ManagedServer server, CancellationToken ct)
        {
            try
            {
                // We notify the server that this document (by its RemoteId) is deleted
                string deleteUrl = $"{server.BaseUrl.TrimEnd('/')}/api/documents/remote/{doc.RemoteId}";
                using var request = new HttpRequestMessage(HttpMethod.Delete, deleteUrl);
                
                if (!string.IsNullOrEmpty(server.AccessToken))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", server.AccessToken);
                }

                var response = await _httpClient.SendAsync(request, ct);
                
                // If success or already gone (404), mark as synced locally
                if (response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await _repository.UpdateSyncStatusAsync(doc.Id, 0, ct: ct);
                    
                    void UpdateLocal()
                    {
                        doc.SyncStatus = 0; // Synced
                    }

                    if (_syncContext != null) _syncContext.Post(_ => UpdateLocal(), null);
                    else UpdateLocal();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Remote delete failed for {doc.Ten}: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Remote delete error for {doc.Ten}: {ex.Message}");
            }
        }

        private async Task HandleDownloadAsync(Document doc, ManagedServer server, CancellationToken ct)
        {
            try
            {
                // Download by RemoteId to be safe
                var response = await SendAuthenticatedRequestAsync(server, HttpMethod.Get, $"api/documents/remote/{doc.RemoteId}/download", null, HttpCompletionOption.ResponseHeadersRead, ct);
                if (response.IsSuccessStatusCode)
                {
                    string targetPath = FileStorageService.ResolvePath(doc.DuongDan);
                    string tempPath = targetPath + ".tmp";
                    string? directory = Path.GetDirectoryName(targetPath);
                    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    using (var stream = await response.Content.ReadAsStreamAsync())
                    using (var fileStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, useAsync: true))
                    {
                        await stream.CopyToAsync(fileStream, 8192, ct);
                    }

                    // Atomic rename
                    if (File.Exists(targetPath)) File.Delete(targetPath);
                    File.Move(tempPath, targetPath);

                    // Update DB status
                    await _repository.UpdateSyncStatusAsync(doc.Id, 0, doc.Version, null, doc.Version, ct);

                    // Update UI object
                    void UpdateLocal()
                    {
                        doc.SyncStatus = 0; // Synced
                        doc.LocalVersion = doc.Version;
                    }

                    if (_syncContext != null)
                    {
                        _syncContext.Post(_ => UpdateLocal(), null);
                    }
                    else
                    {
                        UpdateLocal();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Download failed for {doc.Ten}: {ex.Message}");
            }
        }

        private async Task HandleConflictAsync(Document doc, CancellationToken ct)
        {
            try
            {
                string fullPath = FileStorageService.ResolvePath(doc.DuongDan);
                if (!File.Exists(fullPath)) return;

                string directory = Path.GetDirectoryName(fullPath) ?? "";
                string fileName = Path.GetFileNameWithoutExtension(fullPath);
                string extension = Path.GetExtension(fullPath);
                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                string computerName = Environment.MachineName;
                
                string conflictFileName = $"{fileName}_Conflict_{computerName}_{timestamp}{extension}";
                string conflictPath = Path.Combine(directory, conflictFileName);
                
                File.Copy(fullPath, conflictPath, true);

                int oldStatus = doc.SyncStatus;
                string conflictMsg = $"\n[CONFLICT] Bản sao lưu tại: {conflictFileName}";
                
                // Update DB first
                await _repository.UpdateSyncStatusAsync(doc.Id, 3, null, null, null, ct);

                // Update object on UI thread
                if (_syncContext != null)
                {
                    _syncContext.Send(_ => {
                        doc.SyncStatus = 3; // 3: Conflict
                        if ((doc.GhiChu?.Length ?? 0) + conflictMsg.Length < 2000)
                        {
                            doc.GhiChu += conflictMsg;
                        }
                    }, null);
                }
                else
                {
                    doc.SyncStatus = 3;
                    if ((doc.GhiChu?.Length ?? 0) + conflictMsg.Length < 2000)
                    {
                        doc.GhiChu += conflictMsg;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Conflict resolution failed for {doc.Ten}: {ex.Message}");
            }
        }
    }
}
