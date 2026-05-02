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
    }

    public class SyncEngine : ISyncService
    {
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
        private readonly string _apiUrl; 
        private readonly CancellationTokenSource _cts = new();
        private bool _isRunning = false;

        public SyncEngine(IDocumentRepository repository, string apiUrl)
        {
            _repository = repository;
            if (string.IsNullOrWhiteSpace(apiUrl))
                throw new ArgumentException("API URL must be provided.", nameof(apiUrl));
            
            _apiUrl = apiUrl.TrimEnd('/');
        }

        public void Start()
        {
            if (_isRunning) return;
            _isRunning = true;
            Task.Run(ProcessQueueAsync, _cts.Token);
            
            // Trigger initial sync
            _ = SyncAsync(_cts.Token);
        }

        public void Stop()
        {
            _cts.Cancel();
            _isRunning = false;
        }

        public async Task<Result> SyncAsync(CancellationToken ct = default)
        {
            try
            {
                var documents = await _repository.GetAllByUserIdAsync(UserSession.CurrentUserId, ct);
                foreach (var doc in documents.Where(d => d.SyncStatus == 1)) // 1: PendingUpload
                {
                    Enqueue(doc, SyncType.Upload);
                }

                foreach (var doc in documents.Where(d => d.SyncStatus == 2)) // 2: PendingDownload
                {
                    Enqueue(doc, SyncType.Download);
                }
                
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }

        public async Task<Result<IEnumerable<DocumentDto>>> GetPendingUploadsAsync(CancellationToken ct = default)
        {
             var docs = await _repository.GetAllByUserIdAsync(UserSession.CurrentUserId, ct);
             var dtos = docs.Where(d => d.SyncStatus == 1).Select(d => new DocumentDto { Id = d.Id, FileName = d.Ten });
             return Result<IEnumerable<DocumentDto>>.Success(dtos);
        }

        public async Task<Result<IEnumerable<DocumentDto>>> GetPendingDownloadsAsync(CancellationToken ct = default)
        {
             var docs = await _repository.GetAllByUserIdAsync(UserSession.CurrentUserId, ct);
             var dtos = docs.Where(d => d.SyncStatus == 2).Select(d => new DocumentDto { Id = d.Id, FileName = d.Ten });
             return Result<IEnumerable<DocumentDto>>.Success(dtos);
        }

        public void Enqueue(Document doc, SyncType type)
        {
            string key = $"{doc.Id}_{type}";
            if (_enqueuedTasks.TryAdd(key, 0))
            {
                _taskQueue.Enqueue(new SyncTask { Document = doc, Type = type });
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
                        string key = $"{task.Document.Id}_{task.Type}";
                        _enqueuedTasks.TryRemove(key, out _);
                        
                        await ProcessTaskAsync(task);
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

        private async Task ProcessTaskAsync(SyncTask task)
        {
            var ct = _cts.Token;
            switch (task.Type)
            {
                case SyncType.Upload:
                    await HandleUploadAsync(task.Document, ct);
                    break;
                case SyncType.Download:
                    // TODO: Implement Download
                    break;
            }
        }

        private async Task HandleUploadAsync(Document doc, CancellationToken ct)
        {
            try
            {
                string fullPath = FileStorageService.ResolvePath(doc.DuongDan);
                if (!File.Exists(fullPath)) return;

                // Use MultipartFormDataContent and StreamContent for large file safety (OOM prevention)
                using MultipartFormDataContent content = new();
                content.Add(new StringContent(doc.RemoteId.ToString()), "remoteId");
                content.Add(new StringContent(doc.Version.ToString()), "localVersion");
                if (!string.IsNullOrEmpty(doc.Ten)) content.Add(new StringContent(doc.Ten), "ten");
                if (doc.GhiChu != null) content.Add(new StringContent(doc.GhiChu), "ghiChu");

                using var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 4096, useAsync: true);
                var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                content.Add(fileContent, "file", Path.GetFileName(fullPath));

                var response = await _httpClient.PostAsync($"{_apiUrl}/sync-stream", content, ct);
                
                if (response.IsSuccessStatusCode)
                {
                    var resultJson = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<SyncResponse>(resultJson);
                    
                    if (result?.Success == true)
                    {
                        doc.Version = result.CurrentVersion;
                        doc.SyncStatus = 0; // 0: Synced
                        doc.LocalVersion = doc.Version; 
                        await _repository.UpdateAsync(doc, ct);
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    await HandleConflictAsync(doc);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Upload failed for {doc.Ten}: {ex.Message}");
            }
        }

        private async Task HandleConflictAsync(Document doc)
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

                doc.SyncStatus = 3; // 3: Conflict
                doc.GhiChu += $"\n[CONFLICT] Bản sao lưu tại: {conflictFileName}";
                await _repository.UpdateAsync(doc);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Conflict resolution failed for {doc.Ten}: {ex.Message}");
            }
        }
    }
}
