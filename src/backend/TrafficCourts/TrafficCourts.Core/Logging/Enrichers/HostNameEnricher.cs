using Serilog.Core;
using Serilog.Events;
using System.Runtime.CompilerServices;

namespace TrafficCourts.Logging.Enrichers;

/// <summary>
/// Enriches log events with a HostName property containing current running machines host name.
/// </summary>
/// <remarks>This is based on https://github.com/serilog/serilog-enrichers-environment/blob/dev/src/Serilog.Enrichers.Environment/Enrichers/MachineNameEnricher.cs</remarks>
public class HostNameEnricher : ILogEventEnricher
{
    private LogEventProperty? _cachedProperty { get; set; }

    /// <summary>
    /// The property name added to enriched log events.
    /// </summary>
    public const string HostNamePropertyName = "HostName";

    /// <summary>
    /// Enrich the log event.
    /// </summary>
    /// <param name="logEvent">The log event to enrich.</param>
    /// <param name="propertyFactory">Factory for creating new properties to add to the event.</param>
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        logEvent.AddPropertyIfAbsent(GetLogEventProperty(propertyFactory));
    }

    private LogEventProperty GetLogEventProperty(ILogEventPropertyFactory propertyFactory)
    {
        // Don't care about thread-safety, in the worst case the field gets overwritten and one
        // property will be GCed
        _cachedProperty ??= CreateProperty(propertyFactory);
        return _cachedProperty;
    }

    // Qualify as uncommon-path
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static LogEventProperty CreateProperty(ILogEventPropertyFactory propertyFactory)
    {
        // in Kubernetes, the MachineNameEnricher tries to use Environment.MachineName,
        // but it seems that is always null, so lets use the standard env variable
        var hostName = Environment.GetEnvironmentVariable("HOSTNAME");
        if (string.IsNullOrEmpty(hostName))
        {
            hostName = Environment.MachineName;
        }

        return propertyFactory.CreateProperty(HostNamePropertyName, hostName);
    }
}
