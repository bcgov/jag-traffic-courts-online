using NUnit.Framework;
using Serilog;

namespace TrafficCourts.Common.Configuration.Test
{
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
