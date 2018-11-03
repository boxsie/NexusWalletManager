using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BoxsieApp.Core.Config;
using BoxsieApp.Core.Logging;
using BoxsieApp.Core.Net;
using BoxsieApp.Core.Storage;
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
            var platform = StorageUtils.GetPlatform();
            var filePath = Path.Combine(_walletInstallerPath, $"{_walletConfig.InstallerFilename}{(platform == OS.Windows ? ".exe" : "")}");

            using (var client = _httpClientFactory.GetHttpFileDownload())
            {
                await client.DownloadAsync(_walletConfig.UserConfig.InstallerUrls[platform], filePath, (x) =>
                {
                    var progressBar = StorageUtils.CreateProgressBar(x.Percent, 50);

                    Console.Write(x.Percent < 100
                        ? $"\rDownloading installer.. {progressBar} {Math.Round(x.Percent, 3):N3}% {x.MegabytesPerSecond:N3}Mb/s {x.RemainingTime:hh\\:mm\\:ss} remaining   "
                        : $"\rDownloading installer.. {progressBar} {Math.Round(x.Percent, 3):N3}% {TimeSpan.FromSeconds(x.TotalSeconds):hh\\:mm\\:ss} complete             ");
                });

                Console.WriteLine();
                await _logger.WriteLineAsync($"Nexus {platform} installer download complete", LogLvl.Info);
            }
        }
    }

    public class WalletBootstrap
    {
        private readonly HttpClientFactory _httpClientFactory;

        private readonly WalletConfig _walletConfig;
        private readonly ILog _logger;
        private readonly string _bootstrapFilePath; 
        private readonly string _bootstrapPath;

        public WalletBootstrap(HttpClientFactory httpClientFactory, GeneralConfig generalConfig, WalletConfig walletConfig, ILog logger)
        {
            _httpClientFactory = httpClientFactory;
            _walletConfig = walletConfig;
            _logger = logger;

            _bootstrapFilePath = Path.Combine(generalConfig.UserConfig.UserDataPath, _walletConfig.BootstrapDir, _walletConfig.BootstrapFilename);
            _bootstrapPath = Path.Combine(generalConfig.UserConfig.UserDataPath, _walletConfig.BootstrapDir);
        }

        public async Task CheckForBootstrapAsync()
        {
            if (!Directory.Exists(_bootstrapPath))
                Directory.CreateDirectory(_bootstrapPath);

            if (!File.Exists(_bootstrapFilePath))
                await DownloadAsync();
            else
            {
                using (var client = _httpClientFactory.GetHttpFileDownload())
                {
                    var localSize = new FileInfo(_bootstrapFilePath).Length;
                    var remoteSize = await client.GetFileSizeAsync(_walletConfig.UserConfig.BootstrapUrl);

                    if (localSize != remoteSize)
                        await DownloadAsync();
                }
            }
        }

        private async Task DownloadAsync()
        {
            using (var client = _httpClientFactory.GetHttpFileDownload())
            {
                await client.DownloadAsync(_walletConfig.UserConfig.BootstrapUrl, _bootstrapFilePath, (x) =>
                {
                    var progressBar = StorageUtils.CreateProgressBar(x.Percent, 50);

                    Console.Write(x.Percent < 100
                        ? $"\rDownloading bootstrap.. {progressBar} {Math.Round(x.Percent, 3):N3}% {x.MegabytesPerSecond:N3}Mb/s {x.RemainingTime:hh\\:mm\\:ss} remaining   "
                        : $"\rDownloading bootstrap.. {progressBar} {Math.Round(x.Percent, 3):N3}% {TimeSpan.FromSeconds(x.TotalSeconds):hh\\:mm\\:ss} complete             ");
                });

                Console.WriteLine();
                await _logger.WriteLineAsync("Nexus bootstrap download complete", LogLvl.Info);
            }
        }
    }
}
