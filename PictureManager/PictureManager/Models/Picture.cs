using System;
using System.Diagnostics;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;

namespace PictureManager.Models
{
    class Picture
    {
        private string _path;
        public string CameraModel { get; set; }
        public string DateTaken { get; set; }
        public string[] Tags { get; set; }

        public Picture(string path)
        {
            _path = path;
            RefreshInfo();
        }

        public void SetTags(string tags)
        {
            var picture = ShellObject.FromParsingName(_path);

            try
            {
                var propertyWriter = picture.Properties.GetPropertyWriter();

                propertyWriter.WriteProperty(SystemProperties.System.Comment, tags);
                propertyWriter.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public void SetTag(string tag)
        {
            var picture = ShellObject.FromParsingName(_path);
            var strTags = GetValue(picture.Properties.GetProperty(SystemProperties.System.Comment));

            try
            {
                var propertyWriter = picture.Properties.GetPropertyWriter();

                if (strTags == "")
                {
                    propertyWriter.WriteProperty(SystemProperties.System.Comment, tag);
                }
                else
                {
                    propertyWriter.WriteProperty(SystemProperties.System.Comment, strTags + ";" + tag);
                }

                propertyWriter.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public void RefreshInfo()
        {
            var picture = ShellObject.FromParsingName(_path);
            CameraModel = GetValue(picture.Properties.GetProperty(SystemProperties.System.Photo.CameraModel));
            DateTaken = GetValue(picture.Properties.GetProperty(SystemProperties.System.Photo.DateTaken));
            var strTags = GetValue(picture.Properties.GetProperty(SystemProperties.System.Comment));
            
            if (strTags != null)
            {
                Tags = strTags == "" ? default(string[]) : strTags.Split(';');
            }
        }

        private static string GetValue(IShellProperty value)
        {
            if (value == null || value.ValueAsObject == null)
            {
                return string.Empty;
            }

            return value.ValueAsObject.ToString();
        }
    }
}
