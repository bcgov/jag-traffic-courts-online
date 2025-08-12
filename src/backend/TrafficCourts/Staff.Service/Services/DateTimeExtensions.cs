namespace TrafficCourts.Staff.Service.Services;

public static class DateTimeExtensions
{
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

        return dateTime.Value.UtcToLocalTime(timeZone);
    }

    /// <summary>
    /// Converts a <see cref="DateTime"/> to the specified time zone. The <paramref name="dateTime"/> will be treated as UTC.
    /// </summary>
    /// <param name="dateTime"></param>
    /// <param name="timeZone"></param>
    /// <returns></returns>
    public static DateTime UtcToLocalTime(this DateTime dateTime, TimeZoneInfo timeZone)
    {
        ArgumentNullException.ThrowIfNull(timeZone);
        dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);

        dateTime = TimeZoneInfo.ConvertTime(dateTime, timeZone);

        // Ensure the DateTime is treated as Unspecified to avoid issues with serialization and comparisons
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
}
