using System.Threading.Tasks;
using BoxsieApp.Core;

namespace NexusWalletManager.Core
{
    public class App : IBoxsieApp
    {
        private readonly WalletInstall _install;
        private readonly WalletBootstrap _bootstrap;

        public App(WalletInstall install, WalletBootstrap bootstrap)
        {
            _install = install;
            _bootstrap = bootstrap;
        }

        public async Task StartAsync()
        {
            await _install.CheckForWalletAsync("wallet-1");
            await _bootstrap.CheckForBootstrapAsync();
        }
    }
}