using System.Diagnostics.Metrics;
using TrafficCourts.Diagnostics;

namespace TrafficCourts.Core.Test.Diagnostics;

public class MeterExtensionTest
{
    [Fact]
    public void should_be_able_to_create_timer()
    {
        var meter = new Meter("meter");

        TrafficCourts.Diagnostics.Timer timer = meter.CreateTimer("example-timer", "Example");
        Assert.NotNull(timer);
    }
}
