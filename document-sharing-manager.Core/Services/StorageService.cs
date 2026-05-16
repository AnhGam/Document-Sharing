using System;
using System.IO;
using System.Linq;
using System.Diagnostics;

namespace document_sharing_manager.Core.Services
{
    public static class FileStorageService
    {
        public static string DefaultFolder => 
            document_sharing_manager.Core.Data.UserSession.CurrentUserId > 0 
            ? $"documents_{document_sharing_manager.Core.Data.UserSession.CurrentUserId}" 
            : "documents";

        public static string ResolvePath(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath)) return "";
            
            // If it's already rooted, check if it's within our base directory for safety
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string fullPath;

            if (Path.IsPathRooted(relativePath))
            {
                fullPath = Path.GetFullPath(relativePath);
            }
            else
            {
                fullPath = Path.GetFullPath(Path.Combine(baseDir, relativePath));
            }

            // Path Traversal Check: Ensure the resolved path is still inside the application base directory
            if (!fullPath.StartsWith(baseDir, StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException("Attempted to access path outside of application base directory.");
            }

            return fullPath;
        }

        public static string ImportFile(string sourcePath, string? targetSubFolder = null)
        {
            string folder = targetSubFolder ?? DefaultFolder;
            string fileName = Path.GetFileName(sourcePath);
            string targetDir = ResolvePath(folder);
            if (!Directory.Exists(targetDir)) Directory.CreateDirectory(targetDir);
            string targetPath = Path.Combine(targetDir, fileName);
            File.Copy(sourcePath, targetPath, true);
            return Path.Combine(folder, fileName);
        }

        public static void OpenFile(string path)
        {
            string fullPath = ResolvePath(path);
            if (File.Exists(fullPath))
            {
                Process.Start(new ProcessStartInfo(fullPath) { UseShellExecute = true });
            }
        }

        public static long GetFileSize(string path)
        {
            string fullPath = ResolvePath(path);
            if (File.Exists(fullPath))
            {
                return new FileInfo(fullPath).Length;
            }
            return 0;
        }

        public static string GetFileExtension(string path)
        {
            return Path.GetExtension(path);
        }

        public static bool DeleteFile(string relativePath)
        {
            try
            {
                string fullPath = ResolvePath(relativePath);
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    return true;
                }
            }
            catch { }
            return false;
        }

        public static long GetTotalStorageSize()
        {
            try
            {
                string dataDir = ResolvePath("data");
                if (!Directory.Exists(dataDir)) return 0;
                return Directory.GetFiles(dataDir, "*.*", SearchOption.AllDirectories)
                                .Sum(f => new FileInfo(f).Length);
            }
            catch { return 0; }
        }
    }
}
