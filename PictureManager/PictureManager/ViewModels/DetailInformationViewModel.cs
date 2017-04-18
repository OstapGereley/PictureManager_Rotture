using System.Windows.Input;
using PictureManager.ViewModels.Commands;

namespace PictureManager.ViewModels
{
    public class DetailInformationViewModel : ViewModelBase
    {
        private readonly MainMetroWindowViewModel _mainMetroWindow;

        private bool _isOpen;
        private string _newTagName;
        private TagViewModel _selectedTag;

        public bool IsOpen
        {
            get { return _isOpen; }
            set
            {
                _isOpen = value;
                OnPropertyChanged("IsOpen");
            }
        }
        public string NewTagName
        {
            get { return _newTagName; }
            set
            {
                _newTagName = value;
                OnPropertyChanged("NewTagName");

                RaiseCanExecuteChanges(AddTagCommand as DelegateCommand);
            }
        }

        public TagViewModel SelectedTag
        {
            get { return _selectedTag; }
            set
            {
                _selectedTag = value;
                RaiseCanExecuteChanges(RemoveTagCommand as DelegateCommand);
            }
        }

        public DetailInformationViewModel(MainMetroWindowViewModel mainMetroWindow)
        {
            _mainMetroWindow = mainMetroWindow;

            OpenDetailInformationCommand = new RelayCommand(OpenDetailInformation, CanOpenDetailInformation);
            AddTagCommand = new DelegateCommand(AddTag, CanAddTag);
            RemoveTagCommand = new DelegateCommand(RemoveTag, CanRemoveTag);
        }

        #region Commands

        public ICommand OpenDetailInformationCommand { get; set; }

        public void OpenDetailInformation(object parameter)
        {
            IsOpen = true;
        }

        public bool CanOpenDetailInformation(object parameter)
        {
            return !IsOpen && _mainMetroWindow.SelectedPicture != null;
        }

        public ICommand AddTagCommand { get; set; }

        private void AddTag(object parameter)
        {
            var selectedPicture = _mainMetroWindow.SelectedPicture;
            selectedPicture.AddTag(new TagViewModel {Name = NewTagName});
            NewTagName = null;
        }

        private bool CanAddTag(object parameter)
        {
            if (string.IsNullOrEmpty(NewTagName))
                return false;

            return true;
        }

        public ICommand RemoveTagCommand { get; set; }

        private void RemoveTag(object parameter)
        {
            var selectedPicture = _mainMetroWindow.SelectedPicture;
            selectedPicture.RemoveTag(SelectedTag);
        }

        private bool CanRemoveTag(object parameter)
        {
            if (SelectedTag == null)
                return false;

            return true;
        }

        #endregion
    }
}
