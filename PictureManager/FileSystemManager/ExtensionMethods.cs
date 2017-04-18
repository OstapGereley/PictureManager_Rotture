using System.Configuration;
using System.IO;
using System.Linq;

namespace FileSystemManager
{
    public static class ExtensionMethods
    {
        private static readonly string[] ValidRasterImageFilesExtensions;
        private static readonly string[] ValidDataFilesExtensions;

        static ExtensionMethods()
        {
            var validRasterImageFilesExtensionsPattern = ConfigurationManager.AppSettings["ValidRasterImageFilesExtensions"];
            var validDataFilesExtensionsPattern = ConfigurationManager.AppSettings["ValidDataFilesExtensions"];

            if (validRasterImageFilesExtensionsPattern == null)
                throw new ConfigurationErrorsException("ValidRasterImageFilesExtensions");

            if (validDataFilesExtensionsPattern == null)
                throw new ConfigurationErrorsException("ValidDataFilesExtensions");

            ValidRasterImageFilesExtensions =
                validRasterImageFilesExtensionsPattern.Split('|')
                .Select(ext => ext.ToLower()).ToArray();
            ValidDataFilesExtensions =
                validDataFilesExtensionsPattern.Split('|')
                .Select(ext => ext.ToLower()).ToArray();
        }

        public static bool HasFileTypeExtension(this string path, FileTypes fileType)
        {
            var ext = Path.GetExtension(path);

            if (ext == "") return false;

            switch (fileType)
            {
                case FileTypes.RasterImageFiles:
                    return ValidRasterImageFilesExtensions.Contains(ext.ToLower());
                case FileTypes.DataFiles:
                    return ValidDataFilesExtensions.Contains(ext.ToLower());
                default:
                    return false;
            }
        }

        public static bool IsDirectory(this string path)
        {
            try
            {
                Directory.Exists(path);
            }
            catch (DirectoryNotFoundException)
            {
                return false;
            }

            return true;
        }
    }
}
