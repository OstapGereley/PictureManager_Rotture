using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CloudStorageManager.Tests
{
    [TestClass()]
    public class DropBoxCloudStorageManagerTests
    {
        [TestMethod()]
        public void DropBoxCloudStorageManagerUploadTest()
        {
            var paths = Enumerable.Repeat<string>(@"D:\server\photo\UAAestExp\edit\IMG_0230.jpg",1);
            var DBManager = new DropBoxCloudStorageManager("wpkkoi6x3r4y7b7", "nelrwdtlle9meeb");
            DBManager.Authorize();
            DBManager.CreateConnection("wpkkoi6x3r4y7b7", "nelrwdtlle9meeb");
            try
            {
                DBManager.UploadFilesAsync(paths as string[]);
            }
            catch
            {
                Assert.Fail();
            }
        }

        [TestMethod()]
        public void DropBoxCloudStorageManagerDownloadTest()
        {
            var paths = Enumerable.Repeat<string>(@"/Rotture/logo_icon_64.png", 1);
            var DBManager = new DropBoxCloudStorageManager("wpkkoi6x3r4y7b7", "nelrwdtlle9meeb");
            DBManager.Authorize();
            DBManager.CreateConnection("wpkkoi6x3r4y7b7", "nelrwdtlle9meeb");
            try
            {
                DBManager.DownloadFilesAsync(paths, @"D:\server");
            }
            catch
            {
                Assert.Fail();
            }
        }
    }
}
