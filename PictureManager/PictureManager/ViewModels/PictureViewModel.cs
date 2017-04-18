using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using PictureManager.Models;

namespace PictureManager.ViewModels
{
    public class PictureViewModel : ViewModelBase
    {
        private string _picturePath;
        private Picture _pictureModel;
        private ObservableCollection<TagViewModel> _tags;
        private string _name;

        public PictureViewModel()
        {
            Tags = new ObservableCollection<TagViewModel>();
        }

        public BitmapImage Picture { get; set; }

        public string PicturePath
        {
            get { return _picturePath; }
            set
            {
                if (File.Exists(value))
                {
                    _picturePath = value;
                    Picture = LoadPicture(value);
                    _pictureModel = new Picture(value);
                    Name = Path.GetFileName(value);
                    if (_pictureModel.Tags != null)
                    {
                        Tags = new ObservableCollection<TagViewModel>();

                        foreach (var tagName in _pictureModel.Tags)
                        {
                            Tags.Add(new TagViewModel {Name = tagName});
                        }
                    }
                    OnPropertyChanged("PicturePath");
                }
            }
        }

        public ObservableCollection<TagViewModel> Tags
        {
            get { return _tags; }
            set
            {
                _tags = value;
                OnPropertyChanged("Tags");
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("PictureName");
            }
        }

        private static BitmapImage LoadPicture(string picturePath)
        {
            BitmapImage result = null;

            if (picturePath != null)
            {
                var img = new BitmapImage();
                using (var stream = File.OpenRead(picturePath))
                {
                    img.BeginInit();
                    img.CacheOption = BitmapCacheOption.OnLoad;
                    img.StreamSource = stream;
                    img.EndInit();
                }
                result = img;
            }
            return result;
        }

        public void AddTag(TagViewModel tag)
        {
            Tags.Add(tag);
            _pictureModel.SetTag(tag.Name);
        }

        public void RemoveTag(TagViewModel tag)
        {
            Tags.Remove(tag);

            string tagStr = "";

            _pictureModel.SetTags(tagStr);

            if (Tags.Count == 0)
                return;

            for (int i = 0; i < Tags.Count - 1; i++)
            {
                tagStr += Tags[i].Name + ';';
            }

            tagStr += Tags.Last().Name;

            _pictureModel.SetTag(tagStr);
        }
    }
}
