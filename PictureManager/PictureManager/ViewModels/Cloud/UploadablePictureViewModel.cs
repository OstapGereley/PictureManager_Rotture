using AppLimit.CloudComputing.SharpBox;

namespace PictureManager.ViewModels.Cloud
{
    public class UploadablePictureViewModel : ViewModelBase
    {
        public static bool Cancel = false;

        private bool _readyForUpload;
        private string _picturePath;
        private byte _uploadProgress;
        private bool _isUploaded;

        public bool ReadyForUpload
        {
            get { return _readyForUpload; }
            set
            {
                _readyForUpload = value;
                OnPropertyChanged("ReadyForUpload");
            }
        }

        public string PicturePath
        {
            get { return _picturePath; }
            set
            {
                _picturePath = value;
                OnPropertyChanged("PicturePath");
            }
        }

        public byte UploadProgress
        {
            get { return _uploadProgress; }
            set
            {
                _uploadProgress = value;
                OnPropertyChanged("UploadProgress");
            }
        }

        public bool IsUploaded
        {
            get { return _isUploaded; }
            set
            {
                _isUploaded = value;
                OnPropertyChanged("IsUploaded");
            }
        }

        public void OnUploadProgressChanged(object sender, FileDataTransferEventArgs e)
        {
            UploadProgress = (byte) e.PercentageProgress;

            e.Cancel = Cancel;
        }
    }
}
