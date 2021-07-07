using Moq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Gov.TicketSearch.Auth;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Gov.TicketSearch.Test.Auth
{
    [ExcludeFromCodeCoverage(Justification = Justifications.UnitTestClass)]
    public class OAuthHandlerTest
    {
        [Fact]
#pragma warning disable IDE1006 // Naming Styles
        public void checks_constructor_parameter_is_not_null()
#pragma warning restore IDE1006 // Naming Styles
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new OAuthHandler(null));
            
            var expectedParamName = GetConstructorParameterNames<OAuthHandler>(typeof(ITokenService)).Single();

            Assert.Equal(expectedParamName, actual.ParamName);
        }
        
        [Theory]
        [AutoMockAutoData]
#pragma warning disable IDE1006 // Naming Styles
        public async Task with_token_it_should_add_it_to_header(Token token)
#pragma warning restore IDE1006 // Naming Styles
        {
            Mock<ITokenService> tokenServiceMock = new Mock<ITokenService>();
            tokenServiceMock
                .Setup(x => x.GetTokenAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(token));

            // track to ensure the AssertAuthorization was called
            bool calledAssertAuthorization = false;

            // assert the request authorization has the correct values based on the access token
            void AssertAuthorization(HttpRequestMessage request)
            {
                var authorization = request.Headers.Authorization;
                Assert.Equal(token.AccessToken, authorization?.Parameter);
                Assert.Equal("Bearer", authorization?.Scheme);

                calledAssertAuthorization = true;
            }

            OAuthHandler sut = new OAuthHandler(tokenServiceMock.Object);
            sut.InnerHandler = new TestHandler(AssertAuthorization);

            var invoker = new HttpMessageInvoker(sut);

            var request = new HttpRequestMessage(HttpMethod.Get, "http://foo.com");
            _ = await invoker.SendAsync(request, CancellationToken.None);

            Assert.True(calledAssertAuthorization);
        }

        /// <summary>
        /// Get the names of the matching constructor parameters based on the constructor parameter types
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="types"></param>
        /// <returns></returns>
        private static IEnumerable<string> GetConstructorParameterNames<T>(params Type[] types)
        {
            var constructor = typeof(T).GetConstructor(types);
            Assert.NotNull(constructor);

            var parameterNames = constructor.GetParameters().Select(_ => _.Name);
            return parameterNames;
        }

        /// <summary>
        /// DelegatingHandler that will call user supplied action delegate before calling <see cref="SendAsync"/>.
        /// </summary>
        public class TestHandler : DelegatingHandler
        {
            private readonly Action<HttpRequestMessage> _beforeSendAsync;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="beforeSendAsync">
            /// Action called before calling <see cref="SendAsync"/>. Used to assert properties of the request message.
            /// </param>
            public TestHandler(Action<HttpRequestMessage> beforeSendAsync) : base(new HttpClientHandler())
            {
                _beforeSendAsync = beforeSendAsync ?? throw new ArgumentNullException(nameof(beforeSendAsync));
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                _beforeSendAsync(request);
                return await base.SendAsync(request, cancellationToken);
            }
        }
    }
}
