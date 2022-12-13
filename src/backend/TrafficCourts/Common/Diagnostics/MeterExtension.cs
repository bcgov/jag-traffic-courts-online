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
    /// <remarks>
    /// Timer supports only the following generic parameter types: <see cref="int" /> and <see cref="double" />
    /// </remarks>
    static public Timer<T> CreateTimer<T>(this Meter meter, string name, string? description = null) where T : struct
    {
        return new Timer<T>(meter, name, "ms", description);
    }
}
