using System;
using System.Threading.Tasks;
using NexusWalletManager.Core;

namespace NexusWalletManager.Deamon
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var bs = new WalletBootstrap();

            Task.Run(WalletBootstrap.Download);

            Console.ReadLine();
        }
    }
}
