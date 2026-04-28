using System;
using System.IO;

namespace document_sharing_manager.Core.Services
{
    public static class StorageService
    {
        private static readonly string StorageFolderName = "storage";

        public static string GetStoragePath()
        {
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            string storagePath = Path.Combine(appPath, StorageFolderName);
            
            if (!Directory.Exists(storagePath))
            {
                Directory.CreateDirectory(storagePath);
            }
            
            return storagePath;
        }

        public static string ImportFile(string sourcePath)
        {
            if (string.IsNullOrEmpty(sourcePath) || !File.Exists(sourcePath))
                return sourcePath;

            string fileName = Path.GetFileName(sourcePath);
            string destinationDir = GetStoragePath();
            
            // To avoid name collisions, we can use a timestamp or GUID prefix if needed,
            // but for simplicity, let's try to preserve the name and handle duplicates.
            string destinationPath = Path.Combine(destinationDir, fileName);
            
            if (File.Exists(destinationPath))
            {
                string nameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
                string ext = Path.GetExtension(fileName);
                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                destinationPath = Path.Combine(destinationDir, $"{nameWithoutExt}_{timestamp}{ext}");
            }

            try
            {
                File.Copy(sourcePath, destinationPath, true);
                // Return relative path from app root
                return Path.Combine(StorageFolderName, Path.GetFileName(destinationPath));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error copying file to storage: {ex.Message}");
                return sourcePath; // Fallback to absolute path
            }
        }

        public static string ResolvePath(string storedPath)
        {
            if (string.IsNullOrEmpty(storedPath))
                return storedPath;

            // If it's already an absolute path that exists, return it (legacy support)
            if (Path.IsPathRooted(storedPath) && File.Exists(storedPath))
                return storedPath;

            // Otherwise, resolve relative to app root
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            string absolutePath = Path.Combine(appPath, storedPath);

            if (File.Exists(absolutePath))
                return absolutePath;

            return storedPath; // Return as is if not found
        }
        
        public static long GetTotalStorageSize()
        {
            string storagePath = GetStoragePath();
            DirectoryInfo di = new DirectoryInfo(storagePath);
            long size = 0;
            foreach (FileInfo fi in di.GetFiles("*", SearchOption.AllDirectories))
            {
                size += fi.Length;
            }
            return size;
        }
    }
}
