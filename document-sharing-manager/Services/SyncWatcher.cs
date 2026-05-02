using System;
using System.IO;
using System.Collections.Concurrent;
using System.Timers;
using document_sharing_manager.Core.Services;
using document_sharing_manager.Core.Interfaces;
using document_sharing_manager.Core.Domain;
using System.Linq;
using System.Threading.Tasks;

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
        private readonly ConcurrentDictionary<string, Timer> _debouncers = new();
        private const double DebounceInterval = 3000; // 3 seconds wait after last change

        public SyncWatcher(SyncEngine engine, IDocumentRepository repository)
        {
            _engine = engine;
            _repository = repository;

            string path = FileStorageService.ResolvePath("documents");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            _watcher = new(path)
            {
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size,
                Filter = "*.*",
                IncludeSubdirectories = false
            };

            _watcher.Changed += OnChanged;
            _watcher.Created += OnChanged;
        }

        public void Start()
        {
            _watcher.EnableRaisingEvents = true;
        }

        public void Stop()
        {
            _watcher.EnableRaisingEvents = false;
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            // We only care about file changes, not directories
            if (Directory.Exists(e.FullPath)) return;

            // Debounce to avoid multiple uploads while user is saving
            Debounce(e.FullPath, async () => await HandleFileChangeAsync(e.FullPath));
        }

        private void Debounce(string key, Action action)
        {
            _debouncers.AddOrUpdate(key, 
                k => {
                    var timer = new Timer(DebounceInterval) { AutoReset = false };
                    timer.Elapsed += (s, ev) => {
                        try { action(); }
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
                // Normalize path to match DB entries (which use relative paths)
                string fileName = Path.GetFileName(fullPath);
                string relativePath = Path.Combine("documents", fileName);
                
                // Find document in SQLite using targeted query
                var doc = await _repository.GetByPathAsync(relativePath) ?? 
                          await _repository.GetByPathAsync(fullPath);

                if (doc != null)
                {
                    // Update metadata to trigger sync
                    doc.SyncStatus = 1; // 1: PendingUpload
                    doc.LocalVersion++;
                    await _repository.UpdateAsync(doc);

                    // Signal engine to process
                    _engine.Enqueue(doc, SyncType.Upload);
                    
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
            _watcher.Dispose();
            foreach (var timer in _debouncers.Values)
            {
                timer.Stop();
                timer.Dispose();
            }
            _debouncers.Clear();
        }
    }
}
