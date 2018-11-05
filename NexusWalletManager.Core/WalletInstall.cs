using System;
using System.IO;
using System.Threading.Tasks;
using BoxsieApp.Core.Config;
using BoxsieApp.Core.Logging;
using BoxsieApp.Core.Net;
using BoxsieApp.Core;
using NexusWalletManager.Core.Config;

namespace NexusWalletManager.Core
{
    public class WalletInstall
    {
        private readonly HttpClientFactory _httpClientFactory;
        private readonly WalletConfig _walletConfig;
        private readonly ILog _logger;
        private readonly string _walletInstallerPath;

        public WalletInstall(HttpClientFactory httpClientFactory, GeneralConfig generalConfig, WalletConfig walletConfig, ILog logger)
        {
            _httpClientFactory = httpClientFactory;
            _walletConfig = walletConfig;
            _logger = logger;

            _walletInstallerPath = Path.Combine(generalConfig.UserConfig.UserDataPath, _walletConfig.InstallerDir);
        }

        public async Task CheckForWalletAsync(string name)
        {
            if (!Directory.Exists(_walletInstallerPath))
                Directory.CreateDirectory(_walletInstallerPath);

            await DownloadAsync();
        }

        private async Task DownloadAsync()
        {
            var platform = BoxsieUtils.GetPlatform();
            var url = _walletConfig.UserConfig.InstallerUrls[platform];
            var filePath = Path.Combine(_walletInstallerPath, WalletUtils.GetWalletExecuteableFilename());

            using (var client = _httpClientFactory.GetHttpFileDownload())
            {
                await client.DownloadAsync(url, filePath, (x) =>
                {
                    var progressBar = BoxsieUtils.CreateProgressBar(x.Percent, 50);

                    Console.Write(x.Percent < 100
                        ? $"\rDownloading installer.. {progressBar} {Math.Round(x.Percent, 3):N3}% {x.MegabytesPerSecond:N3}Mb/s {x.RemainingTime:hh\\:mm\\:ss} remaining   "
                        : $"\rDownloading installer.. {progressBar} {Math.Round(x.Percent, 3):N3}% {TimeSpan.FromSeconds(x.TotalSeconds):hh\\:mm\\:ss} complete             ");
                });

                Console.WriteLine();
                await _logger.WriteLineAsync($"Nexus {platform} installer download complete", LogLvl.Info);
            }
        }
    }
}