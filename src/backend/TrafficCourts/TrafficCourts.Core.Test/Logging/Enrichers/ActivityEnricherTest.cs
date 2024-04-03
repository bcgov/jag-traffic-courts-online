using NSubstitute;
using Serilog.Core;
using Serilog.Events;
using Serilog.Parsing;
using System.Diagnostics;
using TrafficCourts.Logging.Enrichers;

namespace TrafficCourts.Core.Test.Logging.Enrichers;

public class ActivityEnricherTest : IDisposable
{
    private ActivityListener _listener;
    private readonly List<Activity> activities = new List<Activity>();
    private ActivitySource _activitySource = new ActivitySource(nameof(ActivityEnricherTest));

    public ActivityEnricherTest()
    {
        // setup the activity listener

        _listener = new ActivityListener();
        _listener.ShouldListenTo = s => s.Name == nameof(ActivityEnricherTest);
        _listener.Sample = (ref ActivityCreationOptions<ActivityContext> options) => ActivitySamplingResult.AllData;
        _listener.ActivityStopped += a => activities.Add(a);
        ActivitySource.AddActivityListener(_listener);
    }

    [Fact]
    public void adds_TraceId_property()
    {
        using var activity = _activitySource.StartActivity("testing");

        ActivityEnricher sut = new ActivityEnricher();

        // create our log event
        LogEvent logEvent = new LogEvent(
            DateTimeOffset.UtcNow,
            LogEventLevel.Information,
            null,
            new MessageTemplate([]),
            []);

        Assert.False(logEvent.Properties.ContainsKey("TraceId"));
        
        sut.Enrich(logEvent, Substitute.For<ILogEventPropertyFactory>());

        Assert.True(logEvent.Properties.ContainsKey("TraceId"));
    }

    public void Dispose()
    {
        _listener?.Dispose();
    }
}
