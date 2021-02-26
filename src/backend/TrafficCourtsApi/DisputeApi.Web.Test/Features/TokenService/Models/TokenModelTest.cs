using DisputeApi.Web.Features.TokenService.Models;
using NUnit.Framework;

namespace DisputeApi.Web.Test.Features.TokenService.Models
{
    public class TokenModelTest
    {

        [Test]
        public void can_create_class()
        {
            var token = new Token("abc123");
            Assert.AreEqual(token.JwtToken, "abc123");
        }
    }
}
