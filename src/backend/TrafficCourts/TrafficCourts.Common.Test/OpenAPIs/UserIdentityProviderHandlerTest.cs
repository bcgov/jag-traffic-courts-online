using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TrafficCourts.Common.OpenAPIs.OracleDataAPI;
using Xunit;

namespace TrafficCourts.Common.Test.OpenAPIs
{
    public class UserIdentityProviderHandlerTest
    {
        [Fact]
        public void throws_ArgumentNullException_when_HttpContextAccessor_is_null()
        {
            Mock<ILogger<UserIdentityProviderHandler>> mockLogger = new();

            var expected = GetParameterName(0);
            var actual = Assert.Throws<ArgumentNullException>(() => new UserIdentityProviderHandler(null!, mockLogger.Object));
            
            Assert.Equal(expected, actual.ParamName);
        }

        [Fact]
        public void throws_ArgumentNullException_when_logger_is_null()
        {
            Mock<IHttpContextAccessor> mockHttpContextAccessor = new();

            var expected = GetParameterName(1);
            var actual = Assert.Throws<ArgumentNullException>(() => new UserIdentityProviderHandler(mockHttpContextAccessor.Object, null!));
            Assert.Equal(expected, actual.ParamName);
        }


        [Fact]
        public async Task request_headers_not_set_when_HttpContext_is_null()
        {
            Mock<IHttpContextAccessor> mockHttpContextAccessor = new();
            Mock<ILogger<UserIdentityProviderHandler>> mockLogger = new();

            HttpRequestMessage request = new();

            var mockDelegatingHandler = CreateMockDelegatingHandler(request);

            var sut = new UserIdentityProviderHandler(mockHttpContextAccessor.Object, mockLogger.Object)
            {
                InnerHandler = mockDelegatingHandler.Object
            };

            var messageInvoker = new HttpMessageInvoker(sut);

            // act
            await messageInvoker.SendAsync(request, default);

            // assert
            Assert.False(request.Headers.TryGetValues("x-username", out _));
            Assert.False(request.Headers.TryGetValues("x-fullName", out _));

            // should log this error
            mockLogger.Verify(l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                // This confirms that the correct log message was sent to the logger. {OriginalFormat} should match the value passed to the logger
                It.Is<It.IsAnyType>((state, type) => CheckValue(state, "Cannot set x-username header, no HttpContext is available, the request not executing part of a HTTP web api request", "{OriginalFormat}")),
                null, // exception
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()), Times.Exactly(1));
        }

        [Fact]
        public async Task request_headers_not_set_when_username_not_present()
        {
            Fixture fixture = new();
            var expected = fixture.Create<ClaimsPrincipalData>();

            DefaultHttpContext context = new();
            context.User = new(new ClaimsIdentity(expected.AuthenticationType));

            Mock<IHttpContextAccessor> mockHttpContextAccessor = new();
            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);

            Mock<ILogger<UserIdentityProviderHandler>> mockLogger = new();

            HttpRequestMessage request = new();

            var mockDelegatingHandler = CreateMockDelegatingHandler(request);

            var sut = new UserIdentityProviderHandler(mockHttpContextAccessor.Object, mockLogger.Object)
            {
                InnerHandler = mockDelegatingHandler.Object
            };

            var messageInvoker = new HttpMessageInvoker(sut);

            // act
            await messageInvoker.SendAsync(request, default);

            // assert
            Assert.False(request.Headers.TryGetValues("x-username", out _));
            Assert.False(request.Headers.TryGetValues("x-fullName", out _));

            // should log this error
            mockLogger.Verify(l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                // This confirms that the correct log message was sent to the logger. {OriginalFormat} should match the value passed to the logger
                It.Is<It.IsAnyType>((state, type) => CheckValue(state, "Could not find preferred_username claim on current user", "{OriginalFormat}")),
                null, // exception
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()), Times.Exactly(1));
        }

        [Fact]
        public async Task request_headers_set_when_username_and_full_name_set()
        {
            // arrange

            // arrange the HttpContext and current user
            Fixture fixture = new();
            var expected = fixture.Create<ClaimsPrincipalData>();

            DefaultHttpContext context = new();
            context.User = new(new ClaimsIdentity(new Claim[] {
                                        new Claim(UserIdentityProviderHandler.UsernameClaimType, expected.Username!),
                                        new Claim(UserIdentityProviderHandler.FullNameClaimType, expected.FullName!)
                                   }, expected.AuthenticationType));

            // arrange the request and a mock handler returning OK
            HttpRequestMessage request = new();
            var mockDelegatingHandler = CreateMockDelegatingHandler(request);

            Mock<IHttpContextAccessor> mockHttpContextAccessor = new();
            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);

            Mock<ILogger<UserIdentityProviderHandler>> mockLogger = new();

            var sut = new UserIdentityProviderHandler(mockHttpContextAccessor.Object, mockLogger.Object)
            {
                InnerHandler = mockDelegatingHandler.Object
            };

            var messageInvoker = new HttpMessageInvoker(sut);

            // act
            await messageInvoker.SendAsync(request, default);

            // assert
            var values = request.Headers.GetValues("x-username").ToList();
            Assert.Single(values);
            Assert.Equal(expected.Username, values[0]);

            values = request.Headers.GetValues("x-fullName").ToList();
            Assert.Single(values);
            Assert.Equal(expected.FullName, values[0]);
        }

        /// <summary>
        /// Creates a mock <see cref="DelegatingHandler"/> that returns Ok in the SendAsync function.
        /// </summary>
        /// <param name="request">The expected request that will be passed to in the SendAsync function.</param>
        private Mock<DelegatingHandler> CreateMockDelegatingHandler(HttpRequestMessage request)
        {
            var mock = new Mock<DelegatingHandler>(MockBehavior.Strict);
            mock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", request, ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            return mock;
        }        

        private static string GetParameterName(int index)
        {
            System.Reflection.ConstructorInfo constructor = typeof(UserIdentityProviderHandler)
                .GetConstructors(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)
                .Single();

            var parameters = constructor.GetParameters();
            return parameters[index].Name!;
        }

        /// <summary>
        /// Checks that a given key exists in the given collection, and that the value matches 
        /// </summary>
        private static bool CheckValue(object state, object expectedValue, string key)
        {
            var keyValuePairList = (IReadOnlyList<KeyValuePair<string, object>>)state;

            var actualValue = keyValuePairList.First(kvp => string.Compare(kvp.Key, key, StringComparison.Ordinal) == 0).Value;

            return expectedValue.Equals(actualValue);
        }

        private class ClaimsPrincipalData
        {
            public string? Username { get; set; }
            public string? FullName { get; set; }
            public string? AuthenticationType { get; set; }
        }
    }
}
