using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.NUnit3;
using DisputeApi.Web.Auth;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace DisputeApi.Web.Test.Auth
{
    [ExcludeFromCodeCoverage]
    public class TokenServiceTest
    {
        [Test,TokenServiceData]
        public async Task GetTokenAsync_if_token_not_expired_return_token_from_memory(
            [Frozen]Mock<IMemoryCache> memoryMock, 
            TokenService sut, 
            Token expectedToken,
            CancellationToken cancellationToken)
        {
            object memoryToken = expectedToken;
            memoryMock
                .Setup(x => x.TryGetValue(It.IsAny<object>(), out memoryToken))
                .Returns(true);

            Token token = await sut.GetTokenAsync(cancellationToken);
            Assert.AreEqual(expectedToken, token);
        }

        [Test,TokenServiceData]
        public async Task GetTokenAsync_if_token_expired_get_new_token_from_client(
            [Frozen] Mock<IMemoryCache> memoryMock,
            [Frozen] Mock<IOAuthClient> authClientMock,
            TokenService sut,
            Token expectedToken,
            CancellationToken cancellationToken
        )
        {
            object expectedMemToken = null;
            memoryMock
                .Setup(x => x.TryGetValue(It.IsAny<object>(), out expectedMemToken))
                .Returns(true);
            authClientMock
                .Setup(x => x.GetRefreshToken(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(expectedToken));

            Token token = await sut.GetTokenAsync(cancellationToken);
            Assert.AreEqual(expectedToken, token);
        }

        internal class TokenServiceDataAttribute : AutoDataAttribute
        {
            public TokenServiceDataAttribute()
              : base(() => new Fixture().Customize(new AutoMoqCustomization()))
            {
            }
        }
    }
}
