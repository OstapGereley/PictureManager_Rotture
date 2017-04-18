using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PictureManager.SocialNetworkAccess.Tests
{
    [TestClass()]
    public class VkWallPostTests
    {
        [TestMethod()]
        public void AddWallPostTest()
        {
            VkWallPost.GetCode();
            VkWallPost.GetToken("https://oauth.vk.com/blank.html#code=90362ffacbdab58a0f");
            VkWallPost.AddWallPost("TEST", @"D:\server\photo\UAAestExp\edit\IMG_0230.jpg");
        }
    }
}
