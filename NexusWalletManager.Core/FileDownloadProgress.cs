using System;

namespace NexusWalletManager.Core
{
    public class FileDownloadProgress
    {
        public long TotalBytes { get; set; }
        public long CurrentBytes { get; set; }
        public double TotalSeconds { get; set; }

        public double SpeedPerSecond(int factor)
        {
            return Math.Round((CurrentBytes / TotalSeconds) / factor, 3);
        }

        public double Percent()
        {
            return ((double)CurrentBytes / TotalBytes) * 100;
        }
    }
}