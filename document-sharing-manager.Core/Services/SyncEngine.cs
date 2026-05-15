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
        public Document Document { get; set; } = null!;
        public SyncType Type { get; set; }
        public int ServerId { get; set; }
    }

    public class SyncEngine(IDocumentRepository repository) : ISyncService, IDisposable
    {
        public event EventHandler? SyncCompleted;
        private readonly IDocumentRepository _repository = repository;
        
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
        private readonly List<ManagedServer> _servers = [.. DatabaseHelper.GetManagedServers().Where(s => s.IsActive)];
        private readonly CancellationTokenSource _cts = new();
        private readonly SynchronizationContext _syncContext = SynchronizationContext.Current;
        private bool _isRunning = false;

        public void AddServer(ManagedServer server)
        {
            if (!_servers.Any(s => s.Id == server.Id))
            {
                _servers.Add(server);
                // Trigger immediate sync for this new server
                _ = SyncServerAsync(server, _cts.Token);
            }
        }

        public void Start()
        {
            if (_isRunning) return;
            _isRunning = true;
            Task.Run(ProcessQueueAsync, _cts.Token);
            
            // Trigger initial sync for all servers
            foreach (var server in _servers)
            {
                _ = SyncServerAsync(server, _cts.Token);
            }
        }

        public void Stop()
        {
            _cts.Cancel();
            _isRunning = false;
        }

        public void Dispose()
        {
            Stop();
            _cts.Dispose();
            _signal.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task<Result> SyncAsync(CancellationToken ct = default)
        {
            var tasks = _servers.Select(s => SyncServerAsync(s, ct));
            await Task.WhenAll(tasks);
            return Result.Success();
        }

        public async Task<Result> SyncServerAsync(ManagedServer server, CancellationToken ct = default)
        {
            try
            {
                // 1. Pull from this specific server
                await PullFromServerAsync(server, ct);

                // 2. Enqueue pending tasks for this server
                // Note: GetPendingSyncDocumentsAsync should be updated to filter by ServerId if needed
                // But for now, we'll filter here
                var documents = await _repository.GetPendingSyncDocumentsAsync(UserSession.CurrentUserId, ct);
                foreach (var doc in documents.Where(d => d.SyncStatus == 1 && (d.ServerId == server.Id || d.ServerId == null)))
                {
                    if (doc.ServerId == null)
                    {
                        doc.ServerId = server.Id;
                        await _repository.UpdateAsync(doc, ct); // Persist the server assignment
                    }
                    Enqueue(doc, SyncType.Upload, server.Id);
                }

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }

        public async Task<Result> PullFromServerAsync(ManagedServer server, CancellationToken ct = default)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, $"{server.BaseUrl}/api/documents");
                if (!string.IsNullOrEmpty(server.AccessToken))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", server.AccessToken);
                }

                var response = await _httpClient.SendAsync(request, ct);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var serverDocs = JsonConvert.DeserializeObject<List<Document>>(json);
                    if (serverDocs == null) return Result.Failure("Failed to parse server response.");

                    foreach (var serverDoc in serverDocs)
                    {
                        var localDoc = await _repository.GetByRemoteIdAsync(serverDoc.RemoteId, ct);
                        if (localDoc == null)
                        {
                            // New document from server - reset local identity
                            serverDoc.Id = 0; 
                            serverDoc.SyncStatus = 2; // PendingDownload
                            serverDoc.LocalVersion = 0; // Haven't downloaded file content yet
                            
                            // Map DuongDan to local pattern
                            serverDoc.DuongDan = Path.Combine(FileStorageService.DefaultFolder, serverDoc.Ten);
                            serverDoc.ServerId = server.Id;
                            
                            await _repository.AddAsync(serverDoc, ct);
                            Enqueue(serverDoc, SyncType.Download, server.Id);
                        }
                        else if (serverDoc.Version > localDoc.Version)
                        {
                            // Newer version on server
                            localDoc.Version = serverDoc.Version;
                            localDoc.Ten = serverDoc.Ten;
                            localDoc.DinhDang = serverDoc.DinhDang;
                            localDoc.GhiChu = serverDoc.GhiChu;
                            localDoc.SyncStatus = 2; // PendingDownload
                            localDoc.ServerId = server.Id;
                            await _repository.UpdateAsync(localDoc, ct);
                            Enqueue(localDoc, SyncType.Download, server.Id);
                        }
                    }
                    if (serverDocs.Count > 0)
                    {
                        if (_syncContext != null) _syncContext.Post(_ => SyncCompleted?.Invoke(this, EventArgs.Empty), null);
                        else SyncCompleted?.Invoke(this, EventArgs.Empty);
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
                _taskQueue.Enqueue(new SyncTask { Document = doc, Type = type, ServerId = serverId });
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
                        var server = _servers.FirstOrDefault(s => s.Id == task.ServerId);
                        if (server == null) continue;

                        string taskKey = $"{task.Document.Id}_{task.Type}_{task.ServerId}";
                        try
                        {
                            if (task.Type == SyncType.Upload)
                                await HandleUploadAsync(task.Document, server, _cts.Token);
                            else if (task.Type == SyncType.Download)
                                await HandleDownloadAsync(task.Document, server, _cts.Token);
                            
                            // Trigger UI refresh if needed
                            _syncContext?.Post(_ => SyncCompleted?.Invoke(this, EventArgs.Empty), null);
                        }
                        finally
                        {
                            _enqueuedTasks.TryRemove(taskKey, out _);
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

                // Use MultipartFormDataContent and StreamContent for large file safety (OOM prevention)
                using MultipartFormDataContent content = new()
                {
                    { new StringContent(doc.RemoteId.ToString()), "remoteId" },
                    { new StringContent(doc.Version.ToString()), "localVersion" }
                };
                if (!string.IsNullOrEmpty(doc.Ten)) content.Add(new StringContent(doc.Ten), "ten");
                if (doc.GhiChu != null) content.Add(new StringContent(doc.GhiChu), "ghiChu");

                using var request = new HttpRequestMessage(HttpMethod.Post, $"{server.BaseUrl}/sync-stream")
                {
                    Content = content
                };

                if (!string.IsNullOrEmpty(server.AccessToken))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", server.AccessToken);
                }

                using var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 4096, useAsync: true);
                var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                content.Add(fileContent, "file", Path.GetFileName(fullPath));

                var response = await _httpClient.SendAsync(request, ct);
                
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

        private async Task HandleDownloadAsync(Document doc, ManagedServer server, CancellationToken ct)
        {
            try
            {
                // Download by RemoteId to be safe
                string downloadUrl = $"{server.BaseUrl}/remote/{doc.RemoteId}/download";
                using var request = new HttpRequestMessage(HttpMethod.Get, downloadUrl);
                
                if (!string.IsNullOrEmpty(server.AccessToken))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", server.AccessToken);
                }

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);
                if (response.IsSuccessStatusCode)
                {
                    string targetPath = FileStorageService.ResolvePath(doc.DuongDan);
                    string? directory = Path.GetDirectoryName(targetPath);
                    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    using (var stream = await response.Content.ReadAsStreamAsync())
                    using (var fileStream = new FileStream(targetPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, useAsync: true))
                    {
                        await stream.CopyToAsync(fileStream, 8192, ct);
                    }

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
