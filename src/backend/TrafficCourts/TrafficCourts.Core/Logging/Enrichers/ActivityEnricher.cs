using Serilog.Core;
using Serilog.Events;
using System.Diagnostics;

namespace TrafficCourts.Logging.Enrichers;

public sealed class ActivityEnricher : ILogEventEnricher
{
    private readonly ActivityEnricherOptions _options;

    public ActivityEnricher() : this(new ActivityEnricherOptions(true, false, false))
    {
    }

    public ActivityEnricher(ActivityEnricherOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        _options = options;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var activity = Activity.Current;
        if (activity is null)
        {
            return;
        }

        if (_options.LogTraceId) AddPropertyIfAbsent(logEvent, activity, "TraceId", GetTraceId);
        if (_options.LogSpanId) AddPropertyIfAbsent(logEvent, activity, "SpanId", GetSpanId);
        if (_options.LogParentId) AddPropertyIfAbsent(logEvent, activity, "ParentId", GetParentId);
    }

    private void AddPropertyIfAbsent(LogEvent logEvent, Activity activity, string propertyName, Func<Activity, string?> valueFactory)
    {
        string? propertyValue = valueFactory(activity);
        if (propertyValue is not null)
        {
            logEvent.AddPropertyIfAbsent(new LogEventProperty(propertyName, new ScalarValue(propertyValue)));
        }
    }

    private static string? GetTraceId(Activity activity)
    {
        ArgumentNullException.ThrowIfNull(activity);

        var traceId = activity.IdFormat switch
        {
            ActivityIdFormat.Hierarchical => activity.RootId,
            ActivityIdFormat.W3C => activity.TraceId.ToHexString(),
            ActivityIdFormat.Unknown => null,
            _ => null,
        };

        return traceId;
    }

    private static string? GetSpanId(Activity activity)
    {
        var spanId = activity.IdFormat switch
        {
            ActivityIdFormat.Hierarchical => activity.Id,
            ActivityIdFormat.W3C => activity.SpanId.ToHexString(),
            ActivityIdFormat.Unknown => null,
            _ => null,
        };

        return spanId;
    }

    private static string? GetParentId(Activity activity)
    {
        var parentId = activity.IdFormat switch
        {
            ActivityIdFormat.Hierarchical => activity.ParentId,
            ActivityIdFormat.W3C => activity.ParentSpanId.ToHexString(),
            ActivityIdFormat.Unknown => null,
            _ => null,
        };

        return parentId;
    }
}
