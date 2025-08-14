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
        ArgumentNullException.ThrowIfNull(timeZone);

        if (dateTime == null)
        {
            return null;
        }

        return dateTime.Value.UtcToLocalTime(timeZone);
    }

    public static DateTime UtcToLocalTime(this DateTime dateTime, TimeZoneInfo timeZone)
    {
        ArgumentNullException.ThrowIfNull(timeZone);
        dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);

        dateTime = TimeZoneInfo.ConvertTime(dateTime, timeZone);

        dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);

        return dateTime;
    }

    public static DateTime? LocalToUtcTime(this DateTime? dateTime, TimeZoneInfo timeZone)
    {
        ArgumentNullException.ThrowIfNull(timeZone);
        if (dateTime == null)
        {
            return null;
        }

        return dateTime.Value.LocalToUtcTime(timeZone);
    }

    public static DateTime LocalToUtcTime(this DateTime dateTime, TimeZoneInfo timeZone)
    {
        ArgumentNullException.ThrowIfNull(timeZone);
        dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);

        dateTime = TimeZoneInfo.ConvertTimeToUtc(dateTime, timeZone);

        // Ensure the DateTime is treated as Unspecified to avoid issues with serialization and comparisons
        dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
        return dateTime;
    }

    /// <summary>
    /// Truncates a <see cref="DateTime"/> to the nearest second.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static DateTime Truncate(this DateTime value)
    {
        // Truncate to the nearest second
        return new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second, value.Kind);
    }
}
