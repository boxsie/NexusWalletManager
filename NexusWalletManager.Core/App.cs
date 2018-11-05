using System.Threading.Tasks;
using BoxsieApp.Core;

namespace NexusWalletManager.Core
{
    public class App : IBoxsieApp
    {
        private readonly WalletService _walletService;

        public App(WalletService walletService)
        {
            _walletService = walletService;
        }

        public async Task StartAsync()
        {
            await _walletService.StartAsync();
        }
    }
}