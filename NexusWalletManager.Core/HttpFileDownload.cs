using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace NexusWalletManager.Core
{
    public class HttpFileDownload : IDisposable
    {
        private HttpClient _client;

        public HttpFileDownload()
        {
            _client = new HttpClient {Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite)};
        }

        public async Task DownloadAsync(string fileUrl, string fileSavePath, string fileSaveName, Action<FileDownloadProgress> progressUpdate)
        {
            try
            {
                using (var response = await _client.GetAsync(fileUrl, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();

                    var fileProgress = new FileDownloadProgress {TotalBytes = response.Content.Headers.ContentLength ?? 0};
                        
                    using (var contentStream = await response.Content.ReadAsStreamAsync())
                    {
                        var buffer = new byte[1024 * 8];
                        var stopwatch = new Stopwatch();

                        var fullPath = Path.Combine(fileSavePath, fileSaveName);

                        using (var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, buffer.Length, true))
                        {
                            stopwatch.Start();

                            var bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length);

                            while (bytesRead > 0)
                            {
                                await fileStream.WriteAsync(buffer, 0, bytesRead);

                                fileProgress.CurrentBytes += bytesRead;
                                fileProgress.TotalSeconds = stopwatch.Elapsed.TotalSeconds;

                                bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length);

                                progressUpdate(fileProgress);
                            }

                            stopwatch.Stop();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void Dispose()
        {
            _client = null;
        }
    }
}