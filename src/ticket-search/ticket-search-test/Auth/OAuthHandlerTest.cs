using Moq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Xunit;
using Gov.TicketSearch.Auth;

namespace Gov.TicketSearch.Test.Auth
{
    public class OAuthHandlerTest
    {
        public class TestHandler : DelegatingHandler
        {
            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                Assert.Equal("accessToken", request.Headers?.Authorization?.Parameter);
                Assert.Equal("Bearer", request.Headers?.Authorization?.Scheme);
                return await base.SendAsync(request, cancellationToken);
            }
        }

        [Theory]
        [AutoMockAutoData]
#pragma warning disable IDE1006 // Naming Styles
        public async Task with_token_it_should_add_it_to_header(            
#pragma warning restore IDE1006 // Naming Styles
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
