namespace TrafficCourts.Domain.Events;

/// <summary>
/// Raised when something about a dispute is changed.
/// </summary>
/// <param name="id"></param>
public class DisputeChangedEvent(long id) : BaseDisputeEvent(id)
{
}
