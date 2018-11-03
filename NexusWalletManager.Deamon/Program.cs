using System;
using System.Threading.Tasks;
using BoxsieApp.Core;
using NexusWalletManager.Core;

namespace NexusWalletManager.Deamon
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            BoxsieAppStartup.Start<AppStartup, App>();

            Console.Read();
        }
    }
}
