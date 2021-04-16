using AutoFixture.NUnit3;
using DisputeApi.Web.Auth;
using DisputeApi.Web.Test.Utils;
using Moq;
using NUnit.Framework;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DisputeApi.Web.Test.Auth
{
    public class OAuthHandlerTest
    {
        public class TestHandler : DelegatingHandler
        {
            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                Assert.AreEqual("accessToken", request.Headers?.Authorization?.Parameter);
                Assert.AreEqual("Bearer", request.Headers?.Authorization?.Scheme);
                return await base.SendAsync(request, cancellationToken);
            }
        }

        [Test, AutoMockAutoData]
        public async Task with_token_it_should_add_it_to_header(            
            [Frozen]Mock<ITokenService> tokenServiceMock,
            OAuthHandler sut,
            Token token
        )
        {
            token.AccessToken = "accessToken";
            var request = new HttpRequestMessage(HttpMethod.Get, "http://foo.com");
            tokenServiceMock.Setup(x => x.GetTokenAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<Token>(token));                
            sut.InnerHandler = new TestHandler { InnerHandler = new HttpClientHandler() };
            var invoker = new HttpMessageInvoker(sut);
            var result = await invoker.SendAsync(request, CancellationToken.None);
        }
    }
}
