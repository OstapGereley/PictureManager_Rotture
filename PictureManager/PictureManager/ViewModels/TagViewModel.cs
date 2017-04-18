using System.Runtime.Serialization;

namespace PictureManager.ViewModels
{
    [DataContract(IsReference = true)]
    public class TagViewModel : ITreeViewElementViewModel
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public CategoryViewModel Parent { get; set; }
    }
}
