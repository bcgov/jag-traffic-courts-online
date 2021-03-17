using System;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using Serilog;
using TrafficCourts.Common.Configuration;

namespace TrafficCourts.Common.Test.Configuration
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class SerilogLoggingTest
    {
        [Test]
        public void GetDefaultLogger_returns_a_logger()
        {
            ILogger actual = SerilogLogging.GetDefaultLogger<SerilogLoggingTest>();

            Assert.NotNull(actual);
        }

        [Test]
        public void GetDefaultLogger_returns_a_logger_in_development()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
            ILogger actual = SerilogLogging.GetDefaultLogger<SerilogLoggingTest>();

            Assert.NotNull(actual);
        }
    }
}
