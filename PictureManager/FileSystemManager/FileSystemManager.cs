using System.IO;
using System.Linq;
using SearchOption = System.IO.SearchOption;

namespace FileSystemManager
{
    public enum FileTypes : byte
    {
        RasterImageFiles,
        DataFiles,
        AllFiles
    }

    public class FileSystemManager
    {
        public static void OpenFileInExplorer(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException(string.Format("File '{0}' does not exist.", filePath));

            var argument = @"/select, " + filePath;

            System.Diagnostics.Process.Start("explorer.exe", argument);
        }

        public static string[] GetFilesByType(string directoryPath, FileTypes fileType, SearchOption searchOption)
        {
            if (!Directory.Exists(directoryPath))
                throw new DirectoryNotFoundException(string.Format("Directory '{0}' does not exist.", directoryPath));

            var files = Directory.GetFiles(directoryPath, "*.*", searchOption);

            switch (fileType)
            {
                case FileTypes.RasterImageFiles:
                    files = files.Where(f => f.HasFileTypeExtension(FileTypes.RasterImageFiles)).ToArray();
                    break;
                case FileTypes.DataFiles:
                    files = files.Where(f => f.HasFileTypeExtension(FileTypes.DataFiles)).ToArray();
                    break;
            }

            return files;
        }

        public static ulong GetDirectorySize(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
                throw new DirectoryNotFoundException(string.Format("Directory '{0}' does not exist.", directoryPath));

            var dirInfo = new DirectoryInfo(directoryPath);

            var files = dirInfo.GetFiles();
            var directories = dirInfo.GetDirectories();

            ulong size = 0;

            foreach (var file in files)
            {
                size += (ulong) file.Length;
            }

            foreach (var dir in directories)
            {
                size += GetDirectorySize(dir.FullName);
            }

            return size;
        }

        public static void DeleteFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException(string.Format("File '{0}' does not exist", filePath));

            File.Delete(filePath);
        }

    }
}

