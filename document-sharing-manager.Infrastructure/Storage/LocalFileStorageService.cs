using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using document_sharing_manager.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace document_sharing_manager.Infrastructure.Storage
{
    public class LocalFileStorageService : IStorageService
    {
        private readonly string _basePath;
        private readonly long _maxFileSizeBytes;

        public LocalFileStorageService(IConfiguration configuration, IHostEnvironment env)
        {
            var configPath = configuration["Storage:LocalBasePath"];
            _basePath = Path.GetFullPath(configPath ?? Path.Combine(env.ContentRootPath, "uploads"));
            
            // Limit capacity: Default 500MB as per documentation
            var maxMb = configuration.GetValue<long>("Storage:MaxFileSizeMB", 500);
            _maxFileSizeBytes = maxMb * 1024 * 1024;

            if (!Directory.Exists(_basePath))
            {
                Directory.CreateDirectory(_basePath);
            }
        }

        public async Task<string> UploadFileAsync(Stream stream, string fileName, string subDirectory = "documents", CancellationToken ct = default)
        {
            // Security: Limit capacity check (Seekable streams)
            if (stream.CanSeek && stream.Length > _maxFileSizeBytes)
            {
                throw new InvalidOperationException($"File size exceeds the maximum limit of {_maxFileSizeBytes / (1024 * 1024)}MB");
            }

            // Security: Prevent Path Traversal by generating a new name
            var extension = Path.GetExtension(fileName);
            var safeFileName = $"{Guid.NewGuid()}{extension}";
            
            var targetDir = Path.Combine(_basePath, subDirectory);
            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }

            var relativePath = Path.Combine(subDirectory, safeFileName);
            var fullPath = Path.Combine(_basePath, relativePath);

            try
            {
                using (var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, useAsync: true))
                {
                    if (stream.CanSeek)
                    {
                        // Check size for seekable streams first
                        if (stream.Length > _maxFileSizeBytes)
                        {
                            throw new InvalidOperationException($"File size exceeds the maximum limit of {_maxFileSizeBytes / (1024 * 1024)}MB");
                        }
                        await stream.CopyToAsync(fileStream, 8192, ct);
                    }
                    else
                    {
                        // For non-seekable streams, check size during copy
                        var buffer = new byte[8192];
                        long totalBytesRead = 0;
                        int bytesRead;
                        while ((bytesRead = await stream.ReadAsync(buffer.AsMemory(), ct)) > 0)
                        {
                            totalBytesRead += bytesRead;
                            if (totalBytesRead > _maxFileSizeBytes)
                            {
                                throw new InvalidOperationException($"File size exceeds the maximum limit of {_maxFileSizeBytes / (1024 * 1024)}MB");
                            }
                            await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), ct);
                        }
                    }
                }
            }
            catch
            {
                // Cleanup partial file on ANY exception (disk error, cancellation, etc.)
                if (File.Exists(fullPath))
                {
                    try { File.Delete(fullPath); } catch { /* Ignore cleanup errors */ }
                }
                throw;
            }

            return relativePath;
        }

        public Task<Stream> GetFileAsync(string path, CancellationToken ct = default)
        {
            var fullPath = GetValidatedFullPath(path);

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException("File not found in local storage", fullPath);
            }

            // Return stream for caller to handle. Use FileShare.Read to allow multiple reads.
            var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read, 8192, useAsync: true);
            return Task.FromResult<Stream>(stream);
        }

        public Task<bool> DeleteFileAsync(string path, CancellationToken ct = default)
        {
            var fullPath = GetValidatedFullPath(path);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<bool> ExistsAsync(string path, CancellationToken ct = default)
        {
            var fullPath = GetValidatedFullPath(path);
            return Task.FromResult(File.Exists(fullPath));
        }

        private string GetValidatedFullPath(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));

            var root = Path.GetFullPath(_basePath);
            if (!root.EndsWith(Path.DirectorySeparatorChar.ToString())) root += Path.DirectorySeparatorChar;

            var fullPath = Path.GetFullPath(Path.Combine(root, path));
            
            if (!fullPath.StartsWith(root, StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException("Access denied: Path is outside storage root.");
            }

            return fullPath;
        }
    }
}
