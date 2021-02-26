using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DisputeApi.Web.Features.TokenService.Models;
using DisputeApi.Web.Features.TokenService.Service;
using DisputeApi.Web.Test.Utils;
using DisputeApi.Web.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace DisputeApi.Web.Test.Features.TokenService.Services
{
    public class TokenServiceTest
    {
        private ITokensService _service;
        private readonly Mock<ILogger<TokensService>> _loggerMock = LoggerServiceMock.LoggerMock<TokensService>();

        [SetUp]
        public void SetUp()
        {
            AppSettings app = new AppSettings()
            {
                JwtExpiry = 800,
                JwtTokenKey = "rtete45354534534vnvng"
            };
            var appSettingsMock = new Mock<IOptions<AppSettings>>();
            appSettingsMock.Setup(ap => ap.Value).Returns(app);
            _service = new TokensService(_loggerMock.Object, appSettingsMock.Object);
        }

        [Test]
        public async Task get_Tokens()
        {
            var result = await _service.CreateToken();
            Assert.IsInstanceOf<Token>(result);
            _loggerMock.VerifyLog(LogLevel.Information, "Created new token", Times.Once());
        }
    }
}
