using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace document_sharing_manager.Core.Interfaces
{
    /// <summary>
    /// Interface for handling file storage operations (Local, S3, MinIO, etc.)
    /// </summary>
    public interface IStorageService
    {
        /// <summary>
        /// Saves a file from a stream.
        /// </summary>
        /// <returns>The relative path of the saved file.</returns>
        Task<string> UploadFileAsync(Stream stream, string fileName, string subDirectory = "documents", CancellationToken ct = default);

        /// <summary>
        /// Retrieves a file stream by its relative path.
        /// </summary>
        Task<Stream> GetFileAsync(string path, CancellationToken ct = default);

        /// <summary>
        /// Deletes a file by its relative path.
        /// </summary>
        Task<bool> DeleteFileAsync(string path, CancellationToken ct = default);

        /// <summary>
        /// Checks if a file exists.
        /// </summary>
        Task<bool> ExistsAsync(string path, CancellationToken ct = default);
    }
}
