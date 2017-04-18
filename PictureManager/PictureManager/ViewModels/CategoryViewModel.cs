using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

namespace PictureManager.ViewModels
{
    [DataContract]
    public class CategoryViewModel : ViewModelBase, ITreeViewElementViewModel
    {
        private ObservableCollection<TagViewModel> _tags;
        
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public ObservableCollection<TagViewModel> Tags
        {
            get { return _tags; }
            set
            {
                _tags = value;
                OnPropertyChanged("Tags");
            }
        }


        public CategoryViewModel()
        {
            Tags = new ObservableCollection<TagViewModel>();
        }
    }
}
