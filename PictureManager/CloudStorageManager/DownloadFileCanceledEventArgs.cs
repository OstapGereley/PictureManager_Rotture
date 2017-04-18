using System;

namespace CloudStorageManager
{
    public class DownloadFileCanceledEventArgs : EventArgs
    {
        public string DownloadedFileName { get; private set; }

        public DownloadFileCanceledEventArgs(string downloadedFileName)
        {
            DownloadedFileName = downloadedFileName;
        }
    }
}
