using NodaTime;

namespace TrafficCourts.Common;

/// <summary>
/// Provides extension to <see cref="IClock"/>.
/// </summary>
public static class ClockExtensions
{
    /// <summary>
    /// America/Vancouver <see cref="https://nodatime.org/TimeZones"/>
    /// </summary>
    private static readonly DateTimeZone _vancouver = DateTimeZoneProviders.Tzdb["America/Vancouver"];

    /// <summary>
    /// Gets the current date and time in pacific time zone.
    /// </summary>
    /// <param name="clock"></param>
    /// <returns></returns>
    public static DateTimeOffset GetCurrentPacificTime(this IClock clock)
    {
        ArgumentNullException.ThrowIfNull(clock);
        Instant now = clock.GetCurrentInstant();
        DateTimeOffset pacificTime = now.InZone(_vancouver).ToDateTimeOffset();
        return pacificTime;
    }
}
