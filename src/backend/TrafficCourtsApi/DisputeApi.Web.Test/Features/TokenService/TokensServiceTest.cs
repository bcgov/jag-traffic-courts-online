using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DisputeApi.Web.Features.TokenService.Models;
using DisputeApi.Web.Features.TokenService.Service;
using DisputeApi.Web.Test.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
            var inMemorySettings = new Dictionary<string, string> {
                {"Jwt.TokenKey", "sdfsdfsdw233434224334"}, {"Jwt.Expiry", "700"}
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
            _service = new TokensService(_loggerMock.Object, configuration);
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
