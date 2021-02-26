using DisputeApi.Web.Utils;
using NUnit.Framework;

namespace DisputeApi.Web.Test.Utils.AppSettingsModel
{
    public class AppSettingsModelTest
    {

        [Test]
        public void can_create_class()
        {
            var settings = new AppSettings();
            settings.JwtExpiry = 30;
            settings.JwtTokenKey = "abc12345";
            Assert.AreEqual(settings.JwtTokenKey, "abc12345");
        }
    }
}
