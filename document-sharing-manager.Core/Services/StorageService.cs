using System;
using System.IO;
using System.Diagnostics;

namespace document_sharing_manager.Core.Services
{
    public static class FileStorageService
    {
        public static string ResolvePath(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath)) return "";
            if (Path.IsPathRooted(relativePath)) return relativePath;
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
        }

        public static string ImportFile(string sourcePath, string targetSubFolder = "documents")
        {
            string fileName = Path.GetFileName(sourcePath);
            string targetDir = ResolvePath(targetSubFolder);
            if (!Directory.Exists(targetDir)) Directory.CreateDirectory(targetDir);
            string targetPath = Path.Combine(targetDir, fileName);
            File.Copy(sourcePath, targetPath, true);
            return Path.Combine(targetSubFolder, fileName);
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

        public static long GetTotalStorageSize()
        {
            // Placeholder
            return 0;
        }
    }
}
