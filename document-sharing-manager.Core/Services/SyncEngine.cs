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
        private readonly HttpClient _httpClient;
        private readonly ConcurrentQueue<SyncTask> _taskQueue = new();
        private readonly SemaphoreSlim _signal = new(0);
        private readonly string _apiUrl = "http://localhost:5247/api/documents"; 
        private readonly CancellationTokenSource _cts = new();
        private bool _isRunning = false;

        public SyncEngine(IDocumentRepository repository)
        {
            _repository = repository;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
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
                // 1. Scan for PendingUploads
                var documents = await _repository.GetAllByUserIdAsync(UserSession.CurrentUserId, ct);
                foreach (var doc in documents.Where(d => d.SyncStatus == 1)) // 1: PendingUpload
                {
                    Enqueue(doc, SyncType.Upload);
                }

                // 2. Scan for PendingDownloads (Status 2)
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
            // Avoid duplicate tasks for the same document in the queue
            if (!_taskQueue.Any(t => t.Document.Id == doc.Id && t.Type == type))
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
                        await ProcessTaskAsync(task);
                    }
                }
                catch (OperationCanceledException) { break; }
                catch (Exception ex)
                {
                    // Basic logging
                    System.Diagnostics.Debug.WriteLine($"SyncEngine Worker Error: {ex.Message}");
                    await Task.Delay(5000, _cts.Token); // Backoff on error
                }
            }
        }

        private async Task ProcessTaskAsync(SyncTask task)
        {
            switch (task.Type)
            {
                case SyncType.Upload:
                    await HandleUploadAsync(task.Document);
                    break;
                case SyncType.Download:
                    // TODO: Implement Download if server provides direct download
                    break;
            }
        }

        private async Task HandleUploadAsync(Document doc)
        {
            try
            {
                string fullPath = FileStorageService.ResolvePath(doc.DuongDan);
                if (!File.Exists(fullPath)) return;

                byte[] fileData = File.ReadAllBytes(fullPath);
                string base64Content = Convert.ToBase64String(fileData);

                var request = new SyncRequest
                {
                    DocumentId = doc.Id,
                    LocalVersion = doc.Version, // Last known server version
                    Ten = doc.Ten,
                    GhiChu = doc.GhiChu,
                    Content = base64Content
                };

                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_apiUrl}/sync", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var resultJson = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<SyncResponse>(resultJson);
                    
                    if (result != null && result.Success)
                    {
                        doc.Version = result.CurrentVersion;
                        doc.SyncStatus = 0; // 0: Synced
                        doc.LocalVersion = doc.Version; // Align local version
                        await _repository.UpdateAsync(doc);
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
