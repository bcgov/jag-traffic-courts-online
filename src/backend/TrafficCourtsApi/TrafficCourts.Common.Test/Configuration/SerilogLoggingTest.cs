using System;
using System.Diagnostics.CodeAnalysis;
using Serilog;
using TrafficCourts.Common.Configuration;
using Xunit;

namespace TrafficCourts.Common.Test.Configuration
{
    [ExcludeFromCodeCoverage]
    public class SerilogLoggingTest
    {
        [Fact]
        public void GetDefaultLogger_returns_a_logger()
        {
            ILogger actual = SerilogLogging.GetDefaultLogger<SerilogLoggingTest>();

            Assert.NotNull(actual);
        }

        [Fact]
        public void GetDefaultLogger_returns_a_logger_in_development()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
            ILogger actual = SerilogLogging.GetDefaultLogger<SerilogLoggingTest>();

            Assert.NotNull(actual);
        }
    }
}
