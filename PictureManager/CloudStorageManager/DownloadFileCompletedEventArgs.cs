using System;

namespace CloudStorageManager
{
    public class DownloadFileCompletedEventArgs : EventArgs
    {
        public string DownloadedFileName { get; private set; }

        public DownloadFileCompletedEventArgs(string downloadedFileName)
        {
            DownloadedFileName = downloadedFileName;
        }
    }
}
