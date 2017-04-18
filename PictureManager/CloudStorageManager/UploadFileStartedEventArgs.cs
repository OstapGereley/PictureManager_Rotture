using System;

namespace CloudStorageManager
{
    public class UploadFileStartedEventArgs : EventArgs
    {
        public string UploadedFileName { get; private set; }

        public UploadFileStartedEventArgs(string uploadedFileName)
        {
            UploadedFileName = uploadedFileName;
        }
    }
}

