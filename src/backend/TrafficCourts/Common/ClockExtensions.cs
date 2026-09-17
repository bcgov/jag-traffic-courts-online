
namespace TrafficCourts.Common;

/// <summary>
/// Provides extension to <see cref="IClock"/>.
/// </summary>
public static class ClockExtensions
{
    /// <summary>
    /// Time zone used when the TZ environment variable is not set (e.g. local development).
    /// <see cref="https://nodatime.org/TimeZones"/>
    /// </summary>
    private const string DefaultTimeZoneId = "America/Vancouver";

    /// <summary>
    /// Returns the current time converted to the time zone configured via the TZ environment
    /// variable (injected by the container runtime), falling back to <see cref="DefaultTimeZoneId"/>
    /// when it is not set.
    /// </summary>
    public static DateTimeOffset GetCurrentConfiguredTime(this TimeProvider clock)
    {
        ArgumentNullException.ThrowIfNull(clock);
        string timeZoneId = Environment.GetEnvironmentVariable("TZ") ?? DefaultTimeZoneId;
        TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        DateTimeOffset configuredTime = TimeZoneInfo.ConvertTime(clock.GetUtcNow(), timeZone);
        return configuredTime;
    }
}
