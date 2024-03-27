using System;

namespace TrafficCourts.Staff.Service.Models;

public class Lock
{
    private readonly TimeProvider _timeProvider;

    public Lock() : this(TimeProvider.System)
    {
    }

    internal Lock(TimeProvider timeProvider)
    {
        ArgumentNullException.ThrowIfNull(timeProvider);
        CreatedAtUtc = timeProvider.GetUtcNow();
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// Lock ID
    /// </summary>
    public string LockId { get; set; } = string.Empty;

    /// <summary>
    /// The ticket number associated with the dispute.
    /// </summary>
    public string TicketNumber { get; set; } = string.Empty;

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
    public DateTimeOffset CreatedAtUtc { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    public bool IsExpired => ExpiryTimeUtc < _timeProvider.GetUtcNow();
}
