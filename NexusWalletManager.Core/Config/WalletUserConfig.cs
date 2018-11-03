using System.Collections.Generic;
using BoxsieApp.Core.Config.Contracts;
using BoxsieApp.Core.Storage;

namespace NexusWalletManager.Core.Config
{
    public class WalletUserConfig : IUserConfig
    {
        public Dictionary<OS, string> InstallerUrls { get; set; }
        public string BootstrapUrl { get; set; }

        public void SetDefault()
        {
            InstallerUrls = new Dictionary<OS, string>
            {
                { OS.Windows, "https://github.com/Nexusoft/Nexus/releases/download/2.5.3/nexus-win-cli.exe" },
                { OS.OSX, "https://github.com/Nexusoft/Nexus/releases/download/2.5.3/nexus-mac-cli"},
                { OS.Linux, "https://github.com/Nexusoft/Nexus/releases/download/2.5.3/nexus-linux-cli" }
            };

            BootstrapUrl = "https://nexusearth.com/bootstrap/LLD-Database/recent.zip";
        }
    }
}