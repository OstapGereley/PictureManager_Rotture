using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using PictureManager.SocialNetworkAccess;
using PictureManager.ViewModels.Commands;
using PictureManager.Views;

namespace PictureManager.ViewModels
{
    public class ImageControlsViewModel : ViewModelBase
    {
        private readonly MainMetroWindowViewModel _mainMetroWindow;

        public DetailInformationViewModel DetailInfo { get; set; }

        public ImageControlsViewModel(MainMetroWindowViewModel mainMetroWindow)
        {
            _mainMetroWindow = mainMetroWindow;
            DetailInfo = new DetailInformationViewModel(mainMetroWindow);

            SwitchPictureLeftCommand = new RelayCommand(SwitchPictureLeft, CanSwitchPictureLeft);
            SwitchPictureRightCommand = new RelayCommand(SwitchPictureRight, CanSwitchPictureRight);
            ActivateFullScreenModeCommand = new DelegateCommand(p => _mainMetroWindow.IsFullScreenModeActivated = true);
            ActivateDefaultScreenModeCommand = new DelegateCommand(p => _mainMetroWindow.IsFullScreenModeActivated = false);
            OpenInExplorerCommand = new RelayCommand(OpenInExplorer, IsPictureSelected);
            DeletePictureCommand = new RelayCommand(DeletePicture, IsPictureSelected);
            VkPostCommand = new RelayCommand(VkPost ,IsPictureSelected);
            OpenCloudStorageCommand = new DelegateCommand(OpenCloudStorage);
        }

        #region Commands
        public ICommand SwitchPictureLeftCommand { get; set; }

        private void SwitchPictureLeft(object parameter)
        {
            var index = _mainMetroWindow.Pictures.IndexOf(_mainMetroWindow.SelectedPicture);

            _mainMetroWindow.SelectedPicture = _mainMetroWindow.Pictures[index - 1];
          
        }

        private bool CanSwitchPictureLeft(object parameter)
        {
            var selectedPicture = _mainMetroWindow.SelectedPicture;

            if (_mainMetroWindow.Pictures.IndexOf(selectedPicture) == 0
                || selectedPicture == null)
                return false;

            return true;
        }

        public ICommand SwitchPictureRightCommand { get; set; }

        private void SwitchPictureRight(object parameter)
        {
            var index = _mainMetroWindow.Pictures.IndexOf(_mainMetroWindow.SelectedPicture);

            _mainMetroWindow.SelectedPicture = _mainMetroWindow.Pictures[index + 1];
        }

        private bool CanSwitchPictureRight(object parameter)
        {
            var selectedPicture = _mainMetroWindow.SelectedPicture;
            
            if (_mainMetroWindow.Pictures.IndexOf(selectedPicture) == _mainMetroWindow.Pictures.Count - 1
                || selectedPicture == null)
                return false;

            return true;
        }

        public ICommand ActivateFullScreenModeCommand { get; set; }

        public ICommand ActivateDefaultScreenModeCommand { get; set; }

        public ICommand OpenInExplorerCommand { get; set; }

        private void OpenInExplorer(object parameter)
        {
            var selectedPicture = _mainMetroWindow.SelectedPicture;

            FileSystemManager.FileSystemManager.OpenFileInExplorer(selectedPicture.PicturePath);
        }

        private bool IsPictureSelected(object parameter)
        {
            var selectedPicture = _mainMetroWindow.SelectedPicture;

            if (selectedPicture == null)
                return false;

            return true;
        }

        public ICommand DeletePictureCommand { get; set; }

        private async void DeletePicture(object parameter)
        {
            var window = parameter as MetroWindow;
            var selectedPicturePath = _mainMetroWindow.SelectedPicture.PicturePath;
          
            var dialogResult = await window.ShowMessageAsync(
                string.Format("Delete Picture"),
                "Are you sure you want to delete this picture?",
                MessageDialogStyle.AffirmativeAndNegative,
                new MetroDialogSettings
                {
                    AffirmativeButtonText = "Yes",
                    NegativeButtonText = "No"
                });

            if (dialogResult == MessageDialogResult.Affirmative)
            {
                FileSystemManager.FileSystemManager.DeleteFile(selectedPicturePath);
            }
        }
        public ICommand VkPostCommand { get; set; }

        private async void VkPost(object parameter)
        {
            var vkpost = new VkWallPost();

            if (VkWallPost.flag == false)
            {
                var window = parameter as MetroWindow;
                vkpost.GetCode();
                var dialogResult = await window.ShowInputAsync(
                    string.Format("VK post"),
                    "In order to post pictures to vk you need to log-in and give program rights to post. " +
                    "In opened browser window copy code and paste here, then press okay.",
                    new MetroDialogSettings
                    {
                        AffirmativeButtonText = "Ok",
                        NegativeButtonText = "Cancel"
                    });
                VkWallPost.Code = dialogResult;
                VkWallPost.flag = true;
                if (!string.IsNullOrEmpty(dialogResult))
                    vkpost.GetToken(VkWallPost.Code);
            }
            if (!string.IsNullOrEmpty(VkWallPost.Code))
            {

                string tempTags = string.Empty;
                var asd = _mainMetroWindow.SelectedPicture.Tags;
                if (asd.Count == 0)
                    tempTags = string.Empty;
                else
                {

                    foreach (var tag in asd)
                    {
                        tempTags += tag.Name;
                        tempTags += ";";
                    }
                }
                Thread myThread = new Thread(() => vkpost.AddWallPost(tempTags, _mainMetroWindow.SelectedPicture.PicturePath));
                myThread.Start();
            }
        }

        public ICommand OpenCloudStorageCommand { get; set; }

        private void OpenCloudStorage(object parameter)
        {

            var cloudStorageViewModel = new CloudStorageViewModel(_mainMetroWindow.Pictures, _mainMetroWindow.TargetFolder);
            var cloudStorageView = new CloudStorageView {DataContext = cloudStorageViewModel};
            cloudStorageView.Show();
        }

        #endregion
    }
}
