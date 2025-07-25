namespace TrafficCourts;

/// <summary>
/// Provides extension to <see cref="IClock"/>.
/// </summary>
public static class ClockExtensions
{
    /// <summary>
    /// America/Vancouver <see cref="https://nodatime.org/TimeZones"/>
    /// </summary>
    private static readonly TimeZoneInfo _vancouver = TimeZoneInfo.FindSystemTimeZoneById("America/Vancouver");

    public static DateTimeOffset GetCurrentPacificTime(this TimeProvider clock)
    {
        ArgumentNullException.ThrowIfNull(clock);
        DateTimeOffset pacificTime = TimeZoneInfo.ConvertTime(clock.GetUtcNow(), _vancouver);
        return pacificTime;
    }

    /// <summary>
    /// Converts a <see cref="DateTime"/> to the specified time zone. The <paramref name="dateTime"/> will be treated as UTC.
    /// </summary>
    /// <param name="dateTime"></param>
    /// <param name="timeZone"></param>
    /// <returns></returns>
    public static DateTime? UtcToLocalTime(this DateTime? dateTime, TimeZoneInfo timeZone)
    {
        if (dateTime == null)
        {
            return null;
        }

        ArgumentNullException.ThrowIfNull(timeZone);
        dateTime = DateTime.SpecifyKind(dateTime.Value, DateTimeKind.Utc);

        return TimeZoneInfo.ConvertTime(dateTime.Value, timeZone);
    }
}
