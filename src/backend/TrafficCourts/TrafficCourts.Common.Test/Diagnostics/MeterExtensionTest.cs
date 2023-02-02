using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TrafficCourts.Common.Test.Diagnostics;

public class MeterExtensionTest
{
    [Fact]
    public void should_be_able_to_create_timer()
    {
        var meter = new Meter("meter");

        Timer<int>? intTimer = meter.CreateTimer<int>("int-timer");
        Assert.NotNull(intTimer);

        Timer<double>? doubleTimer = meter.CreateTimer<double>("double-timer");
        Assert.NotNull(doubleTimer);
    }
}

public class TimerTest
{ 
    [Fact]
    public void create_int_timer()
    {
        var meter = new Meter("meter");

        Timer<int> timer = new Timer<int>(meter, "int-timer", "ms", null);
        using var mark1 = timer.Start();

        var tags = new KeyValuePair<string, object?>("name", "value");
        using var mark2 = timer.Start(tags);
        timer.Stop(mark2);
    }

    [Fact]
    public void create_double_timer()
    {
        var meter = new Meter("meter");

        Timer<double> timer = new Timer<double>(meter, "double-timer", "ms", null);
        using var mark1 = timer.Start();

        var tags = new KeyValuePair<string, object?>("name", "value");
        using var mark2 = timer.Start(tags);
        timer.Stop(mark2);
    }

    [Fact]
    public void create_invalid_timer_with_long()
    {
        var meter = new Meter("meter");

        Timer<long> timer = new Timer<long>(meter, "long-timer", "ms", null);
        var mark1 = timer.Start();

        var tags = new KeyValuePair<string, object?>("name", "value");
        var mark2 = timer.Start(tags);

        InvalidOperationException actual;
        actual = Assert.Throws<InvalidOperationException>(() => timer.Stop(mark2));
        Assert.Equal("System.Int64 is unsupported type for this operation. The only supported types are int and double.", actual.Message);

        // 
        actual = Assert.Throws<InvalidOperationException>(() => mark1.Dispose());
        Assert.Equal("System.Int64 is unsupported type for this operation. The only supported types are int and double.", actual.Message);

    }

    [Fact]
    public void create_invalid_timer_with_float()
    {
        var meter = new Meter("meter");

        Timer<float> timer = new Timer<float>(meter, "long-timer", "ms", null);
        var mark1 = timer.Start();

        var tags = new KeyValuePair<string, object?>("name", "value");
        var mark2 = timer.Start(tags);

        InvalidOperationException actual;
        actual = Assert.Throws<InvalidOperationException>(() => timer.Stop(mark2));
        Assert.Equal("System.Single is unsupported type for this operation. The only supported types are int and double.", actual.Message);

        actual = Assert.Throws<InvalidOperationException>(() => mark1.Dispose());
        Assert.Equal("System.Single is unsupported type for this operation. The only supported types are int and double.", actual.Message);
    }
}

