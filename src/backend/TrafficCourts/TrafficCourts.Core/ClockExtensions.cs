
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
}
