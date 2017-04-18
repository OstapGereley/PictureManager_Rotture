using System;

namespace PictureManager.Models
{
    [Serializable]
    public class PathInfo
    {
        public string Name { get; set; }
        public PathInfo(string name)
        {
            Name = name;
        }
    }
}
