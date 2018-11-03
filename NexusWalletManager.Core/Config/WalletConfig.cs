using System.IO;
using System.Reflection;
using BoxsieApp.Core.Config;
using BoxsieApp.Core.Config.Contracts;

namespace NexusWalletManager.Core.Config
{
    public class WalletConfig : BaseConfig<WalletUserConfig>
    {
        public string InstallerDir { get; set; }
        public string InstallerFilename { get; set; }
        public string BootstrapDir { get; set; }
        public string BootstrapFilename { get; set; }
    }
}