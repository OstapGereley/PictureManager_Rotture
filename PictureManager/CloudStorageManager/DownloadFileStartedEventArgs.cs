using System;

namespace CloudStorageManager
{
    public class DownloadFileStartedEventArgs : EventArgs
    {
        public string DownloadedFileName { get; private set; }

        public DownloadFileStartedEventArgs(string downloadedFileName)
        {
            DownloadedFileName = downloadedFileName;
        }
    }
}
