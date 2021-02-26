using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DisputeApi.Web.Features.TokenService.Controller;
using DisputeApi.Web.Features.TokenService.Models;
using DisputeApi.Web.Features.TokenService.Service;
using DisputeApi.Web.Test.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace DisputeApi.Web.Test.Features.TokenService
{
    public class TokenssControllerTest
    {
        private TokensController _controller;
        private readonly Mock<ILogger<TokensController>> _loggerMock = LoggerServiceMock.LoggerMock<TokensController>();
        private Mock<ITokensService> _tokensServiceMock = new Mock<ITokensService>();

        [SetUp]
        public void SetUp()
        {
            _controller = new TokensController(_loggerMock.Object, _tokensServiceMock.Object);
        }

        [Test]
        public async Task get_Token()
        {
            var Token = new Token("abcd1234");
            _tokensServiceMock.Setup(x => x.CreateToken()).Returns(
              Task.FromResult(new Token("abcd1234")));
            var result = (OkObjectResult)await _controller.GetToken();
            Assert.IsInstanceOf<Token>(result.Value);
            Assert.IsNotNull(result);
            _tokensServiceMock.Verify(x => x.CreateToken(), Times.Once);

        }
    }
}
