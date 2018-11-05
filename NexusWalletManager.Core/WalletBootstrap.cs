using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BoxsieApp.Core.Config;
using BoxsieApp.Core.Data;
using BoxsieApp.Core.Logging;
using BoxsieApp.Core.Net;
using BoxsieApp.Core.Repository;
using BoxsieApp.Core;
using BoxsieApp.Core.Storage;
using NexusWalletManager.Core.Config;

namespace NexusWalletManager.Core
{
    public enum WalletStatus
    {
        OK,
        NotRunning,
        Error
    }

    public class WalletService
    {
        private readonly IRepository<WalletInfo> _walletRepo;

        public WalletService(IRepository<WalletInfo> walletRepo, GeneralConfig config)
        {
            _walletRepo = walletRepo;
        }

        public async Task StartAsync()
        {
            await _walletRepo.CreateTable(nameof(WalletInfo));

            await CreateWalletAsync(new WalletInfo
            {
                Name = "Moop",
                Location = "Moop",
                DataDir = "Moop",
                RpcUsername = "Moop",
                RpcPassword = "Moop",
                CreatedOn = DateTime.Now,
                UpdatedOn = DateTime.Now
            });
        }

        public async Task CreateWalletAsync(WalletInfo walletInfo)
        {
            var existingWallet = await _walletRepo.GetWhereAsync(new List<WhereClause>
            {
                new WhereClause(nameof(WalletInfo.Location), "=", walletInfo.Location, "OR"),
                new WhereClause(nameof(WalletInfo.Name), "=", walletInfo.Name)
            });

            await _walletRepo.CreateAsync(walletInfo);
        }

        public async Task AddExistingWalletAsync()
        {

        }
    }

    public class WalletInfo : IEntity
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public string DataDir { get; set; }
        public string RpcUsername { get; set; }
        public string RpcPassword { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }

    public class Wallet
    {
        public WalletStatus Status { get; set; }
        public WalletInfo Info { get; set; }
    }

    public static class WalletUtils
    {
        public static string BuildConf(this WalletInfo info)
        {
            return $@"rpcuser={info.RpcUsername}
                      rpcpassword={info.RpcPassword}
                      rpcallowip=127.0.0.1";
        }

        public static string GetWalletExecuteableFilename()
        {
            var cfg = Cfg.GetConfig<WalletConfig>();
            var platform = BoxsieUtils.GetPlatform();

            return $"{cfg.InstallerFilename}{(platform == OS.Windows ? ".exe" : "")}";
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
                    var progressBar = BoxsieUtils.CreateProgressBar(x.Percent, 50);

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
