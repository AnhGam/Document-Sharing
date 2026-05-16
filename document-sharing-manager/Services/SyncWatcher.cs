using System;
using System.IO;
using System.Collections.Concurrent;
using System.Timers;
using document_sharing_manager.Core.Services;
using document_sharing_manager.Core.Interfaces;
using document_sharing_manager.Core.Domain;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace document_sharing_manager.Services
{
    /// <summary>
    /// Monitors local file changes and triggers SyncEngine tasks with Debounce logic.
    /// </summary>
    public class SyncWatcher : IDisposable
    {
        private readonly SyncEngine _engine;
        private readonly IDocumentRepository _repository;
        private readonly FileSystemWatcher _watcher;
        private readonly SynchronizationContext _syncContext;
        private readonly ConcurrentDictionary<string, System.Timers.Timer> _debouncers = new();
        private readonly CancellationTokenSource _cts = new();
        private const double DebounceInterval = 3000; // 3 seconds wait after last change

        public SyncWatcher(SyncEngine engine, IDocumentRepository repository)
        {
            _engine = engine;
            _repository = repository;
            _syncContext = SynchronizationContext.Current;

            string path = FileStorageService.ResolvePath(FileStorageService.DefaultFolder);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            _watcher = new(path)
            {
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size,
                Filter = "*.*",
                IncludeSubdirectories = false
            };

            _watcher.Changed += OnChanged;
            _watcher.Created += OnChanged;
            _watcher.Renamed += (s, e) => OnChanged(s, e);
        }

        public void Start()
        {
            _watcher.EnableRaisingEvents = true;
        }

        public void Stop()
        {
            _watcher.EnableRaisingEvents = false;
            _cts.Cancel();
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            // We only care about file changes, not directories
            if (Directory.Exists(e.FullPath)) return;

            // Debounce to avoid multiple uploads while user is saving
            Debounce(e.FullPath, () => HandleFileChangeAsync(e.FullPath));
        }

        private void Debounce(string key, Func<Task> action)
        {
            _debouncers.AddOrUpdate(key, 
                k => {
                    var timer = new System.Timers.Timer(DebounceInterval) { AutoReset = false };
                    timer.Elapsed += async (s, ev) => {
                        try { await action(); }
                        catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Debounce error: {ex.Message}"); }
                        finally {
                            _debouncers.TryRemove(k, out _);
                            timer.Dispose();
                        }
                    };
                    timer.Start();
                    return timer;
                },
                (k, existingTimer) => {
                    existingTimer.Stop();
                    existingTimer.Start();
                    return existingTimer;
                });
        }

        private async Task HandleFileChangeAsync(string fullPath)
        {
            try 
            {
                // Normalize path to match DB entries (which use relative paths like 'documents\file.ext')
                string fileName = Path.GetFileName(fullPath);
                string relativePath = Path.Combine(FileStorageService.DefaultFolder, fileName);
                
                    // Find document in SQLite using single targeted query
                    var doc = await _repository.GetByPathAsync(relativePath, _cts.Token);

                    if (doc != null)
                    {
                        int newLocalVersion = doc.LocalVersion + 1;

                        // 1. Update DB synchronously on background thread
                        await _repository.UpdateSyncStatusAsync(doc.Id, 1, null, null, newLocalVersion, _cts.Token);

                        // 2. Update object on UI thread via Post
                        void UpdateLocal()
                        {
                            doc.SyncStatus = 1; // 1: PendingUpload
                            doc.LocalVersion = newLocalVersion;
                        }

                        if (_syncContext != null)
                        {
                            _syncContext.Send(_ => UpdateLocal(), null);
                        }
                        else
                        {
                            UpdateLocal();
                        }

                        // Signal engine to process if server ID is assigned
                        if (doc.ServerId.HasValue)
                        {
                            _engine.Enqueue(doc, SyncType.Upload, doc.ServerId.Value);
                        }
                        // If ServerId is null, the main sync loop will handle assigning it later.
                    
                    System.Diagnostics.Debug.WriteLine($"File change detected and enqueued: {fileName}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SyncWatcher Error: {ex.Message}");
            }
        }

        public void Dispose()
        {
            Stop();
            _watcher.Dispose();
            _cts.Dispose();
            foreach (var timer in _debouncers.Values)
            {
                timer.Stop();
                timer.Dispose();
            }
            _debouncers.Clear();
        }
    }
}
