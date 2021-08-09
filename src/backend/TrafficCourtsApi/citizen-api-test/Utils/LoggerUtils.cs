using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Gov.CitizenApi.Test.Utils
{
    [ExcludeFromCodeCoverage]
    public static class LoggerServiceMock
    {
        public static Mock<ILogger<T>> LoggerMock<T>() where T : class
        {
            return new Mock<ILogger<T>>();
        }

        /// <summary>
        /// Returns an <pre>ILogger<T></pre> as used by the Microsoft.Logging framework.
        /// You can use this for constructors that require an ILogger parameter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ILogger<T> Logger<T>() where T : class
        {
            return LoggerMock<T>().Object;
        }

        public static void VerifyLog<T>(this Mock<ILogger<T>> loggerMock, LogLevel level, string message, string failMessage = null)
        {
            loggerMock.VerifyLog(level, message, Times.Once());
        }
        public static void VerifyLog<T>(this Mock<ILogger<T>> loggerMock, LogLevel level, string message, Times times)
        {
            loggerMock.Verify(l => l.Log<Object>(level, It.IsAny<EventId>(), It.Is<Object>(o => o.ToString() == message), null, (Func<Object, Exception, string>)It.IsAny<object>()), times, message);
        }
    }
}
