using System;

namespace CloudStorageManager
{
    public class UploadFileCanceledEventArgs : EventArgs
    {
        public string DownloadedFileName { get; private set; }

        public UploadFileCanceledEventArgs(string downloadedFileName)
        {
            DownloadedFileName = downloadedFileName;
        }
    }
}
