using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PictureManager.;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace PictureManager.Tests
{
    [TestClass()]
    public class DropBoxCloudStorageManagerTests
    {
        [TestMethod()]
        public void DropBoxCloudStorageManagerUploadTest()
        {
            var paths = Enumerable.Repeat<string>(@"D:\server\photo\UAAestExp\edit\IMG_0230.jpg", 1);
            var DBManager = new DropBoxCloudStorageManager("wpkkoi6x3r4y7b7", "nelrwdtlle9meeb");
            DBManager.Authorize();
            DBManager.CreateConnection("wpkkoi6x3r4y7b7", "nelrwdtlle9meeb");
            try
            {
                DBManager.UploadFiles(paths);
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
                DBManager.DownloadFiles(paths, @"D:\server");
            }
            catch
            {
                Assert.Fail();
            }
        }
    }
}
