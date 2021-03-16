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
    }
}
