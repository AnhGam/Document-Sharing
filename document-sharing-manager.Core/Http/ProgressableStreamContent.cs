using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace document_sharing_manager.Core.Http
{
    public class ProgressableStreamContent : HttpContent
    {
        private const int DefaultBufferSize = 81920; // 80 KB
        private readonly Stream _content;
        private readonly int _bufferSize;
        private readonly Action<long, long> _progressCallback;

        public ProgressableStreamContent(Stream content, Action<long, long> progressCallback) 
            : this(content, DefaultBufferSize, progressCallback)
        {
        }

        public ProgressableStreamContent(Stream content, int bufferSize, Action<long, long> progressCallback)
        {
            _content = content ?? throw new ArgumentNullException(nameof(content));
            _bufferSize = bufferSize <= 0 ? DefaultBufferSize : bufferSize;
            _progressCallback = progressCallback ?? throw new ArgumentNullException(nameof(progressCallback));
        }

        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            var buffer = new byte[_bufferSize];
            long totalBytes = _content.Length;
            long uploadedBytes = 0;

            using (_content)
            {
                while (true)
                {
                    int length = await _content.ReadAsync(buffer, 0, buffer.Length);
                    if (length <= 0)
                    {
                        break;
                    }

                    uploadedBytes += length;
                    _progressCallback?.Invoke(uploadedBytes, totalBytes);

                    await stream.WriteAsync(buffer, 0, length);
                    await stream.FlushAsync();
                }
            }
        }

        protected override bool TryComputeLength(out long length)
        {
            length = _content.Length;
            return true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _content.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
