using NUnit.Framework;

namespace PictureManager.SocialNetworkAccess.Tests
{
    [TestFixture]
    public class VkWallPostTests
    {
        [Test]
        public void AddWallPostTest()
        {
			var a = new VkWallPost();
            a.GetCode();
            a.GetToken("https://oauth.vk.com/blank.html#code=90362ffacbdab58a0f");
            a.AddWallPost("TEST", @"D:\server\photo\UAAestExp\edit\IMG_0230.jpg");
        }
    }
}
