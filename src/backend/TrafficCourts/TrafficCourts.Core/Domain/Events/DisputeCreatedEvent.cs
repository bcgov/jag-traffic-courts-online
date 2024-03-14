namespace TrafficCourts.Domain.Events;

/// <summary>
/// Rasied when a dispute is created.
/// </summary>
/// <param name="id"></param>
public class DisputeCreatedEvent(long id) : BaseDisputeEvent(id)
{
}

