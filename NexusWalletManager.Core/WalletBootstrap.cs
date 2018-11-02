using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace NexusWalletManager.Core
{
    public class WalletBootstrap
    {
        private const string BootstrapUrl = "https://nexusearth.com/bootstrap/LLD-Database/recent.zip";
        private static string BootstrapSavePath => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private const string BootstrapFilename = "bootstrap.zip";

        public static async Task Download()
        {
            using (var client = new HttpFileDownload())
            {
                await client.DownloadAsync(BootstrapUrl, BootstrapSavePath, BootstrapFilename, (x) =>
                {
                    var pct = x.Percent();
                    var progressBar = LogProgress(pct);
                    var pctText = Math.Round(pct, 3).ToString("N3");

                    Console.Write($"\rDownloading.. {progressBar} {pctText}% {x.SpeedPerSecond((int)1E+6)}Mb/s");
                });
            }
        }

        private static string LogProgress(double percent)
        {
            var progress = Math.Floor((double)percent / 5);
            var bar = "";

            for (var o = 0; o < 20; o++)
            {
                bar += progress > o
                    ? '#'
                    : ' ';
            }

            return $"[{bar}]";
        }
    }
}
