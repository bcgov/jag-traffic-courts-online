namespace TrafficCourts.Domain.Events;

/// <summary>
/// Base event for dispute notifications.
/// </summary>
/// <param name="id"></param>
public abstract class BaseDisputeEvent(long id) : BaseEvent
{
    /// <summary>
    /// The dispute id the event is for.
    /// </summary>
    public long Id { get; } = id;
}
