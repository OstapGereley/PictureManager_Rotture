using System;
using AppLimit.CloudComputing.SharpBox;

namespace PictureManager.ViewModels.Cloud
{
    public class DownloadablePictureViewModel : ViewModelBase
    {
        public static bool Cancel = false;

        private bool _readyForDownload;
        private string _pictureName;
        private byte _downloadProgress;
        private bool _isDownloaded;

        public bool IsDownloaded
        {
            get { return _isDownloaded; }
            set
            {
                _isDownloaded = value;
                OnPropertyChanged("IsDownloaded");
            }
        }

        public bool ReadyForDownload
        {
            get { return _readyForDownload; }
            set
            {
                _readyForDownload = value;
                OnPropertyChanged("ReadyForDownload");
            }
        }

        public string PictureName
        {
            get { return _pictureName; }
            set
            {
                _pictureName = value;
                OnPropertyChanged("PictureName");
            }
        }

        public byte DownloadProgress
        {
            get { return _downloadProgress; }
            set
            {
                _downloadProgress = value;
                OnPropertyChanged("DownloadProgress");
            }
        }

        public void OnDownloadProgressChanged(object sender, FileDataTransferEventArgs e)
        {
            DownloadProgress = (byte) e.PercentageProgress;

            e.Cancel = Cancel;
        }
    }
}
