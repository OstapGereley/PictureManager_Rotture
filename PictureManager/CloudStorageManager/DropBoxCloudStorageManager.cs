using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppLimit.CloudComputing.SharpBox;
using AppLimit.CloudComputing.SharpBox.Exceptions;
using AppLimit.CloudComputing.SharpBox.StorageProvider.DropBox;

namespace CloudStorageManager
{
    public class DropBoxCloudStorageManager
    {
        private readonly DropBoxRequestToken _requestToken;
        private ICloudStorageAccessToken _accessToken;
        private readonly DropBoxConfiguration _configuration;
        private readonly string _authorizationUrl;
        private readonly CloudStorage _cloudStorage;

        /// <summary>
        /// The default value is '/'.
        /// </summary>
        public string ApplicationFolderPath { get; set; }

        public FileOperationProgressChanged DownloadProgressChangedCallback { get; set; }
        public FileOperationProgressChanged UploadProgressChangedCallback { get; set; }

        public event EventHandler<DownloadFileCompletedEventArgs> DownloadFileCompleted;

        public event EventHandler<DownloadFileStartedEventArgs> DownloadFileStarted;

        public event EventHandler<DownloadFileCanceledEventArgs> DownloadFileCanceled;

        protected virtual void OnDownloadFileCanceled(DownloadFileCanceledEventArgs e)
        {
            EventHandler<DownloadFileCanceledEventArgs> handler = DownloadFileCanceled;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnDownloadFileStarted(DownloadFileStartedEventArgs e)
        {
            EventHandler<DownloadFileStartedEventArgs> handler = DownloadFileStarted;
            if (handler != null) handler(this, e);
        }

        private void OnDownloadFileCompleted(DownloadFileCompletedEventArgs e)
        {
            var handler = DownloadFileCompleted;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<UploadFileCompletedEventArgs> UploadFileCompleted;

        public event EventHandler<UploadFileStartedEventArgs> UploadFileStarted;

        public event EventHandler<UploadFileCanceledEventArgs> UploadFileCanceled;

        protected virtual void OnUploadFileCanceled(UploadFileCanceledEventArgs e)
        {
            EventHandler<UploadFileCanceledEventArgs> handler = UploadFileCanceled;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnUploadFileStarted(UploadFileStartedEventArgs e)
        {
            EventHandler<UploadFileStartedEventArgs> handler = UploadFileStarted;
            if (handler != null) handler(this, e);
        }

        private void OnUploadFileCompleted(UploadFileCompletedEventArgs e)
        {
            var handler = UploadFileCompleted;
            if (handler != null) handler(this, e);
        }

        public DropBoxCloudStorageManager(string key, string secret)
        {
            _configuration =
                CloudStorage.GetCloudConfigurationEasy(nSupportedCloudConfigurations.DropBox) as DropBoxConfiguration;
            _configuration.AuthorizationCallBack = new Uri("https://www.dropbox.com/home");

            _requestToken = DropBoxStorageProviderTools.GetDropBoxRequestToken(_configuration, key, secret);

            try
            {
                _authorizationUrl =
                    DropBoxStorageProviderTools.GetDropBoxAuthorizationUrl(_configuration, _requestToken);
            }
            catch (NullReferenceException)
            {
                throw new ArgumentException("Invalid application key and/or secret");
            }

            _cloudStorage = new CloudStorage();
            ApplicationFolderPath = "/";
        }

        /// <summary>
        /// Opens the authorization page only if a user is not authorized.
        /// </summary>
        public void Authorize()
        {
            if (_accessToken == null)
                System.Diagnostics.Process.Start(_authorizationUrl);
        }

        /// <summary>
        /// Creates a connection to the cloud storage.
        /// </summary>
        /// <param name="key">Application key.</param>
        /// <param name="secret">Application secret.</param>
        /// <returns>False if the connection is already established
        /// or a user is not authorized on the website.</returns>
        public bool CreateConnection(string key, string secret)
        {
            if (_accessToken == null)
            {
                try
                {
                    _accessToken =
                        DropBoxStorageProviderTools.ExchangeDropBoxRequestTokenIntoAccessToken(_configuration, key,
                            secret, _requestToken);

                    return true;
                }
                catch (UnauthorizedAccessException)
                {
                    return false;
                }
            }

            return false;
        }

        private bool WaitForResponse(string key, string secret, TimeSpan timeout)
        {
            var time = new TimeSpan();

            while (true)
            {
                if (CreateConnection(key, secret))
                    return true;

                Thread.Sleep(1000);

                time = time.Add(TimeSpan.FromSeconds(1));

                if (time.TotalSeconds >= timeout.TotalSeconds)
                    return false;
            }
        }

        public Task<bool> WaitForResponseAsync(string key, string secret, TimeSpan timeout)
        {
            return Task.Factory.StartNew(() => WaitForResponse(key, secret, timeout));
        }



        private static ulong GetFilesSize(IEnumerable<string> files)
        {
            if (files == null)
                return 0;

            ulong size = 0;

            foreach (var file in files)
            {
                if (File.Exists(file))
                {
                    size += (ulong)new FileInfo(file).Length;
                }
            }

            return size;
        }

        public ulong GetAvailableMemory()
        {
            if (_accessToken == null)
                throw new InvalidOperationException("Connection is not established.");

            var quotaInfo = DropBoxStorageProviderTools.GetAccountInformation(_accessToken).QuotaInfo;

            return quotaInfo.QuotaBytes - quotaInfo.NormalBytes;
        }

        public int UploadFiles(IEnumerable<string> files)
        {
            if (files == null)
                return 0;

            if (_accessToken == null)
                throw new InvalidOperationException("Connection is not established.");

            var uploadedCount = 0;

            _cloudStorage.Open(_configuration, _accessToken);

            var filesSize = GetFilesSize(files as string[]);
            var availableMemory = GetAvailableMemory();

            if (filesSize > availableMemory)
            {
                _cloudStorage.Close();
                throw new OutOfMemoryException(string.Format("Available memory in the cloud storage: {0}",
                    availableMemory));
            }

            if (!DirectoryExists(ApplicationFolderPath))
                _cloudStorage.CreateFolder(ApplicationFolderPath);

            var appFolder = _cloudStorage.GetFolder(ApplicationFolderPath);


            foreach (var file in files)
            {
                if (File.Exists(file))
                {
                    OnUploadFileStarted(new UploadFileStartedEventArgs(file));
                    try
                    {
                        _cloudStorage.UploadFile(file, appFolder, UploadProgressChangedCallback);
                    }
                    catch (SharpBoxException)
                    {
                        OnUploadFileCanceled(new UploadFileCanceledEventArgs(file));

                        return uploadedCount;
                    }
                    uploadedCount++;
                    OnUploadFileCompleted(new UploadFileCompletedEventArgs(file));
                }
            }

            _cloudStorage.Close();

            return uploadedCount;
        }

        public Task<int> UploadFilesAsync(string[] files)
        {
            return Task.Factory.StartNew(() => UploadFiles(files));
        }

        public string[] GetFileNamesAvailableForDownload()
        {
            if (_accessToken == null)
                throw new InvalidOperationException("Connection is not established.");

            if (!DirectoryExists(ApplicationFolderPath))
                return null;

            _cloudStorage.Open(_configuration, _accessToken);

            var appFolder = _cloudStorage.GetFolder(ApplicationFolderPath);

            var fileNamesAvailableForDownload =
                (from file in appFolder
                 where !(file is ICloudDirectoryEntry)
                 select file.Name).ToArray();

            _cloudStorage.Close();

            return fileNamesAvailableForDownload;
        }

        public int DownloadFiles(IEnumerable<string> fileNames, string targetPath)
        {
            if (fileNames == null || !Directory.Exists(targetPath) || !DirectoryExists(ApplicationFolderPath))
                return 0;

            if (_accessToken == null)
                throw new InvalidOperationException("Connection is not established.");

            _cloudStorage.Open(_configuration, _accessToken);

            var downloadedCount = 0;
            var appFolder = _cloudStorage.GetFolder(ApplicationFolderPath);

            foreach (var fileName in fileNames)
            {
                if (FileExists(fileName, ApplicationFolderPath))
                {
                    OnDownloadFileStarted(new DownloadFileStartedEventArgs(fileName));

                    try
                    {
                        _cloudStorage.DownloadFile(appFolder, fileName, targetPath, DownloadProgressChangedCallback);
                    }
                    catch (SharpBoxException) //canceled
                    {
                        var filePath = targetPath + @"\" + fileName;
 
                        OnDownloadFileCanceled(new DownloadFileCanceledEventArgs(filePath));

                        return downloadedCount;
                    }

                    downloadedCount++;
                    OnDownloadFileCompleted(new DownloadFileCompletedEventArgs(fileName));
                }
            }

            _cloudStorage.Close();

            return downloadedCount;
        }

        public Task<int> DownloadFilesAsync(IEnumerable<string> fileNames, string targetPath)
        {
            return Task.Factory.StartNew(() => DownloadFiles(fileNames, targetPath));
        }

        private bool DirectoryExists(string path)
        {
            if (path == null)
                return false;

            if (_accessToken == null)
                throw new InvalidOperationException("Connection is not established.");

            var flag = false;

            if (!_cloudStorage.IsOpened)
            {
                flag = true;
                _cloudStorage.Open(_configuration, _accessToken);
            }

            try
            {
                _cloudStorage.GetFolder(path);
            }
            catch (SharpBoxException)
            {
                return false;
            }

            if (flag)
                _cloudStorage.Close();

            return true;
        }

        private bool FileExists(string fileName, string directoryPath)
        {
            if (fileName == null || directoryPath == null || !DirectoryExists(directoryPath))
                return false;

            if (_accessToken == null)
                throw new InvalidOperationException("Connection is not established.");

            var flag = false;

            if (!_cloudStorage.IsOpened)
            {
                flag = true;
                _cloudStorage.Open(_configuration, _accessToken);
            }

            var dir = _cloudStorage.GetFolder(directoryPath);

            try
            {
                _cloudStorage.GetFile(fileName, dir);
            }
            catch (SharpBoxException)
            {
                return false;
            }

            if (flag)
                _cloudStorage.Close();

            return true;
        }
    }
}