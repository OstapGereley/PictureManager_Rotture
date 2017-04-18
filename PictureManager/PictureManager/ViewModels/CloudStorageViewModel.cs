using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CloudStorageManager;
using FileSystemManager;
using PictureManager.ViewModels.Cloud;
using PictureManager.ViewModels.Commands;

namespace PictureManager.ViewModels
{
    public class CloudStorageViewModel : ViewModelBase
    {
        private string _targetFolder;
        private readonly string _appKey;
        private readonly string _appSecret;
        private readonly DropBoxCloudStorageManager _dropBoxCloudStorageManager;
        
        #region Properties
        private bool _isAuthorizationUrlOpen;
        public bool IsAuthorizationUrlOpen
        {
            get { return _isAuthorizationUrlOpen; }
            set
            {
                _isAuthorizationUrlOpen = value;
                OnPropertyChanged("IsAuthorizationUrlOpen");
               
                RaiseCanExecuteChanges(LoginCommand as DelegateCommand);
            }
        }

        private Visibility _timeoutAlertVisibility;
        public Visibility TimeoutAlertVisibility
        {
            get { return _timeoutAlertVisibility; }
            set
            {
                _timeoutAlertVisibility = value;
                OnPropertyChanged("TimeoutAlertVisibility");
            }
        }

        private bool _isConnected;
        private ObservableCollection<DownloadablePictureViewModel> _downloadablePictures;
        private ObservableCollection<UploadablePictureViewModel> _uploadablePictures;
        private bool _isDownloading;
        private bool _isRefreshing;
        private bool _isUploading;

        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                _isConnected = value;
                OnPropertyChanged("IsConnected");
            }
        }

        public ObservableCollection<DownloadablePictureViewModel> DownloadablePictures
        {
            get { return _downloadablePictures; }
            set
            {
                _downloadablePictures = value;
                OnPropertyChanged("DownloadablePictures");
            }
        }

        public ObservableCollection<UploadablePictureViewModel> UploadablePictures
        {
            get { return _uploadablePictures; }
            set
            {
                _uploadablePictures = value;
                OnPropertyChanged("UploadablePictures");
            }
        }

        public bool IsDownloading
        {
            get { return _isDownloading; }
            set
            {
                _isDownloading = value;
                RaiseCanExecuteChanges(DownloadCommand as DelegateCommand);
                RaiseCanExecuteChanges(CancelDownloadCommand as DelegateCommand);
                RaiseCanExecuteChanges(RefreshDownloadInfoCommand as DelegateCommand);
                RaiseCanExecuteChanges(UploadCommand as DelegateCommand);
            }
        }

        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set
            {
                _isRefreshing = value;
                OnPropertyChanged("IsRefreshing");
            }
        }

        public bool IsUploading
        {
            get { return _isUploading; }
            set
            {
                _isUploading = value;
                RaiseCanExecuteChanges(UploadCommand as DelegateCommand);
                RaiseCanExecuteChanges(DownloadCommand as DelegateCommand);
                RaiseCanExecuteChanges(RefreshDownloadInfoCommand as DelegateCommand);
                RaiseCanExecuteChanges(CancelUploadCommand as DelegateCommand);
            }
        }

        #endregion

        private CloudStorageViewModel()
        {
            _appKey = ConfigurationManager.AppSettings["DropBoxAppKey"];
            _appSecret = ConfigurationManager.AppSettings["DropBoxAppSecret"];

            _dropBoxCloudStorageManager = new DropBoxCloudStorageManager(_appKey, _appSecret)
            {
                ApplicationFolderPath = ConfigurationManager.AppSettings["DropBoxAppFolderPath"]
            };

            TimeoutAlertVisibility = Visibility.Hidden;

            LoginCommand = new DelegateCommand(Login, p => !IsAuthorizationUrlOpen);
            OpenHomePageCommand = new DelegateCommand(o => System.Diagnostics.Process.Start("https://www.dropbox.com/"));
            DownloadCommand = new DelegateCommand(Download, CanDownload);
            CancelDownloadCommand = new DelegateCommand(CancelDownload, CanCancelDownload);
            RefreshDownloadInfoCommand = new DelegateCommand(RefreshDownloadInfo, CanRefreshDownloadInfo);

            UploadCommand = new DelegateCommand(Upload, CanUpload);
            CancelUploadCommand = new DelegateCommand(CancelUpload, CanCancelUpload);

            _dropBoxCloudStorageManager.DownloadFileStarted += OnDownloadStarted;
            _dropBoxCloudStorageManager.DownloadFileCompleted += OnDownloadCompleted;
            _dropBoxCloudStorageManager.DownloadFileCanceled += OnDownloadCanceled;

            _dropBoxCloudStorageManager.UploadFileStarted += OnUploadStarted;
            _dropBoxCloudStorageManager.UploadFileCompleted += OnUploadCompleted;
            _dropBoxCloudStorageManager.UploadFileCanceled += OnUploadCanceled;
        }

        public CloudStorageViewModel(IEnumerable<PictureViewModel> pictures, string targetFolder) : this()
        {
            _targetFolder = targetFolder;
            UploadablePictures = new ObservableCollection<UploadablePictureViewModel>();

            foreach (var picture in pictures)
            {
                UploadablePictures.Add(new UploadablePictureViewModel
                {
                    PicturePath = picture.PicturePath,
                    ReadyForUpload = true
                });
                
            }
        }

        #region Commands
        public ICommand LoginCommand { get; set; }
        public ICommand OpenHomePageCommand { get; set; }

        private async void Login(object parameter)
        {
            _dropBoxCloudStorageManager.Authorize();
            IsAuthorizationUrlOpen = true;
            TimeoutAlertVisibility = Visibility.Hidden;

            IsConnected = await _dropBoxCloudStorageManager.WaitForResponseAsync(_appKey, _appSecret, TimeSpan.FromSeconds(30));

            IsAuthorizationUrlOpen = false;

            if (!IsConnected)
            {
                TimeoutAlertVisibility = Visibility.Visible;
                return;
            }

            IsRefreshing = true;

            DownloadablePictures = await RefreshDownloadInfoAsync();

            IsRefreshing = false;


        }

        public ICommand DownloadCommand { get; set; }

        private async void Download(object parameter)
        {
            DownloadablePictureViewModel.Cancel = false;

            foreach (var downloadablePicture in DownloadablePictures)
            {
                downloadablePicture.DownloadProgress = 0;
                downloadablePicture.IsDownloaded = false;
            }

            IsDownloading = true;

            var downloadedCount = await _dropBoxCloudStorageManager
                .DownloadFilesAsync(DownloadablePictures
                .Where(elem => elem.ReadyForDownload)
                .Select(p => p.PictureName), _targetFolder);

            IsDownloading = false;
        }

        private bool CanDownload(object parameter)
        {
            if (IsDownloading || IsUploading)
                return false;

            return true;
        }

        public ICommand CancelDownloadCommand { get; set; }

        private void CancelDownload(object parameter)
        {
            DownloadablePictureViewModel.Cancel = true;
        }

        private bool CanCancelDownload(object parameter)
        {
            return IsDownloading;
        }

        public ICommand RefreshDownloadInfoCommand { get; set; }

        private async void RefreshDownloadInfo(object parameter)
        {
            IsRefreshing = true;

            DownloadablePictures = await RefreshDownloadInfoAsync();

            IsRefreshing = false;
        }

        private bool CanRefreshDownloadInfo(object parameter)
        {
            if (IsDownloading || IsUploading)
                return false;

            return true;
        }

        public ICommand UploadCommand { get; set; }

        private async void Upload(object parameter)
        {
            UploadablePictureViewModel.Cancel = false;

            foreach (var uploadablePicture in UploadablePictures)
            {
                uploadablePicture.UploadProgress = 0;
                uploadablePicture.IsUploaded = false;
            }

            this.IsUploading = true;

            var uploadedCount = await _dropBoxCloudStorageManager
                .UploadFilesAsync(UploadablePictures
                .Where(elem => elem.ReadyForUpload)
                .Select(p => p.PicturePath).ToArray());

            this.IsUploading = false;
        }

        private bool CanUpload(object parameter)
        {
            if (IsDownloading || IsUploading)
                return false;

            return true;
        }

        public ICommand CancelUploadCommand { get; set; }

        private void CancelUpload(object parameter)
        {
            UploadablePictureViewModel.Cancel = true;
        }

        private bool CanCancelUpload(object parameter)
        {
            return IsUploading;
        }
        #endregion

        private void OnDownloadStarted(object sender, DownloadFileStartedEventArgs e)
        {
            if (DownloadablePictures == null || DownloadablePictures.Count == 0)
                return;
            
            var downloadablePicture =
                DownloadablePictures.FirstOrDefault(elem => elem.PictureName == e.DownloadedFileName);

            if (downloadablePicture != null)
                _dropBoxCloudStorageManager.DownloadProgressChangedCallback = downloadablePicture.OnDownloadProgressChanged;
        }

        private void OnDownloadCompleted(object sender, DownloadFileCompletedEventArgs e)
        {
            var downloadedPicture =
                DownloadablePictures.FirstOrDefault(elem => elem.PictureName == e.DownloadedFileName);

            if (downloadedPicture != null) downloadedPicture.IsDownloaded = true;
        }

        private void OnDownloadCanceled(object sender, DownloadFileCanceledEventArgs e)
        {
            if (File.Exists(e.DownloadedFileName))
                File.Delete(e.DownloadedFileName);
        }

        private ObservableCollection<DownloadablePictureViewModel> RefreshDownloadInfo()
        {
            if (!IsConnected)
                return null;

            var fileNamesAvailableForDownload = _dropBoxCloudStorageManager.GetFileNamesAvailableForDownload();

            var downloadablePictures = new ObservableCollection<DownloadablePictureViewModel>();

            if (fileNamesAvailableForDownload != null)
            {
                foreach (var fileName in fileNamesAvailableForDownload)
                {
                    if (fileName.HasFileTypeExtension(FileTypes.RasterImageFiles))
                    {
                        downloadablePictures.Add(
                            new DownloadablePictureViewModel
                            {
                                PictureName = fileName,
                                ReadyForDownload = true
                            });
                    }
                }
            }

            return downloadablePictures;
        }

        private Task<ObservableCollection<DownloadablePictureViewModel>> RefreshDownloadInfoAsync()
        {
            return Task.Factory.StartNew(() => RefreshDownloadInfo());
        }

        private void OnUploadStarted(object sender, UploadFileStartedEventArgs e)
        {
            if (UploadablePictures == null || UploadablePictures.Count == 0)
                return;

            var uploadablePicture =
                UploadablePictures.FirstOrDefault(elem => elem.PicturePath == e.UploadedFileName);

            if (uploadablePicture != null)
                _dropBoxCloudStorageManager.UploadProgressChangedCallback = uploadablePicture.OnUploadProgressChanged;
        }

        private void OnUploadCompleted(object sender, UploadFileCompletedEventArgs e)
        {
            var uploadablePicture =
                UploadablePictures.FirstOrDefault(elem => elem.PicturePath == e.UploadedFileName);

            if (uploadablePicture != null) uploadablePicture.IsUploaded = true;
        }

        private void OnUploadCanceled(object sender, UploadFileCanceledEventArgs e)
        {
            //
        }
    }
}
