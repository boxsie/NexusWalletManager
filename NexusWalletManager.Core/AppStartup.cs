using System;
using BoxsieApp.Core;
using BoxsieApp.Core.Config;
using BoxsieApp.Core.Config.Contracts;
using BoxsieApp.Core.Repository;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace NexusWalletManager.Core
{
    public class AppStartup : BoxsieAppStartup
    {
        public AppStartup()
        {

        }

        protected override void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IRepository<WalletInfo>, Repository<WalletInfo>>();

            services.AddTransient<WalletService>();
            services.AddTransient<WalletInstall>();
            services.AddTransient<WalletBootstrap>();
        }

        protected override void Configure(IServiceProvider serviceProvider)
        {

        }
    }
}
