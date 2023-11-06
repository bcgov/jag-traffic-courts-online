namespace TrafficCourts.Staff.Service.Models;

public class Lock
{
    /// <summary>
    /// The function that returns the current UTC date and time. Overriden for tests.
    /// </summary>
    private readonly Func<DateTimeOffset> _utcNow;

    public Lock() : this(() => DateTimeOffset.UtcNow)
    {
    }

    internal Lock(Func<DateTimeOffset> utcNow)
    {
        _utcNow = utcNow ?? throw new ArgumentNullException(nameof(utcNow));
        CreatedAtUtc = _utcNow();
    }

    /// <summary>
    /// Lock ID
    /// </summary>
    public string? LockId { get; set; }

    /// <summary>
    /// JJ Dispute ID
    /// </summary>
    public long DisputeId { get; set; }

    /// <summary>
    /// Username of the person who acquired the lock.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// The time in UTC when the acquired lock expires.
    /// </summary>
    public DateTimeOffset ExpiryTimeUtc { get; set; }

    /// <summary>
    /// The time in UTC when the lock was created.
    /// </summary>
    public DateTimeOffset CreatedAtUtc { get; }
}
