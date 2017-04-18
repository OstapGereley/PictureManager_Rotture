using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using FileSystemManager;

namespace PictureManager.ViewModels
{
    public class MainMetroWindowViewModel : ViewModelBase
    {
        public string TargetFolder { get; set; }
        private ObservableCollection<PictureViewModel> _pictures;
        public ObservableCollection<PictureViewModel> Pictures
        {
            get { return _pictures; }
            set
            {
                _pictures = value;
                OnPropertyChanged("Pictures");
            }
        }
        
        private PictureViewModel _selectedPicture;
        public PictureViewModel SelectedPicture
        {
            get { return _selectedPicture; }
            set
            {
                _selectedPicture = value;
                OnPropertyChanged("SelectedPicture");
            }
        }

        private bool _isFullScreenModeActivated;
        public bool IsFullScreenModeActivated
        {
            get { return _isFullScreenModeActivated; }
            set
            {
                _isFullScreenModeActivated = value;
                OnPropertyChanged("IsFullScreenModeActivated");
            }
        }

        public SettingsViewModel Settings { get; set; }
        public CategoriesPanelViewModel CategoriesPanel { get; set; }
        public ImageControlsViewModel ImageControls { get; set; }

        public MainMetroWindowViewModel(string path)
        {
            TargetFolder = path;

            Settings = new SettingsViewModel();
            CategoriesPanel = new CategoriesPanelViewModel(this);
            ImageControls = new ImageControlsViewModel(this);

            Pictures = new ObservableCollection<PictureViewModel>();

            var files = FileSystemManager.FileSystemManager.GetFilesByType(path, FileTypes.RasterImageFiles,
                SearchOption.AllDirectories);

            foreach (var file in files)
            {
                Pictures.Add(new PictureViewModel
                {
                    PicturePath = file
                });
            }

            if (Pictures.Count != 0)
                SelectedPicture = Pictures.First();

            var fileSystemWatcher = new FileSystemWatcher
            {
                Path = path,
                IncludeSubdirectories = true
            };

            fileSystemWatcher.Created += OnChanged;
            fileSystemWatcher.Deleted += OnChanged;

            fileSystemWatcher.EnableRaisingEvents = true;

        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Created)
            {
                var path = e.FullPath;

                if (path.HasFileTypeExtension(FileTypes.RasterImageFiles))
                {
                    //Pictures.AddOnUI(new PictureViewModel {PicturePath = path});
                }
            }
        }
    }
}
