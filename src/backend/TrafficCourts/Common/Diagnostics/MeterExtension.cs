using Timer = TrafficCourts.Common.Diagnostics.Timer;

namespace System.Diagnostics.Metrics;

/// <summary>
/// Provides extension method for creating a timer based on a meter.
/// </summary>
/// <remarks>
/// Taken from https://github.com/open-telemetry/opentelemetry-specification/issues/464
/// </remarks>
public static class MeterExtension
{
    /// <summary>
    /// Create a timer to record  the number of milliseconds an operation takes.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="meter"></param>
    /// <param name="name"></param>
    /// <param name="description"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="meter"/>, <paramref name="name"/> or <paramref name="description"/> is null.
    /// </exception>
    static public Timer CreateTimer(this Meter meter, string name, string description)
    {
        ArgumentNullException.ThrowIfNull(meter);
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(description);

        return new Timer(meter, name, "ms", description);
    }
}
