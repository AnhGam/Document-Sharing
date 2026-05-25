using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;

namespace document_sharing_manager.Core.Http
{
    public class ProgressableStreamContent : HttpContent
    {
        private const int DefaultBufferSize = 81920; // 80 KB
        private readonly Stream _content;
        private readonly int _bufferSize;
        private readonly Action<long, long> _progressCallback;
        private readonly CancellationToken _cancellationToken;

        public ProgressableStreamContent(Stream content, Action<long, long> progressCallback, CancellationToken cancellationToken = default) 
            : this(content, DefaultBufferSize, progressCallback, cancellationToken)
        {
        }

        public ProgressableStreamContent(Stream content, int bufferSize, Action<long, long> progressCallback, CancellationToken cancellationToken = default)
        {
            _content = content ?? throw new ArgumentNullException(nameof(content));
            _bufferSize = bufferSize <= 0 ? DefaultBufferSize : bufferSize;
            _progressCallback = progressCallback ?? throw new ArgumentNullException(nameof(progressCallback));
            _cancellationToken = cancellationToken;
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
                    int length = await _content.ReadAsync(buffer, 0, buffer.Length, _cancellationToken);
                    if (length <= 0)
                    {
                        break;
                    }

                    uploadedBytes += length;
                    _progressCallback?.Invoke(uploadedBytes, totalBytes);

                    await stream.WriteAsync(buffer, 0, length, _cancellationToken);
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
