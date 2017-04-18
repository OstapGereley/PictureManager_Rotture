using System;

namespace CloudStorageManager
{
    public class UploadFileCompletedEventArgs : EventArgs
    {
        public string UploadedFileName { get; private set; }

        public UploadFileCompletedEventArgs(string uploadedFileName)
        {
            UploadedFileName = uploadedFileName;
        }
    }
}
