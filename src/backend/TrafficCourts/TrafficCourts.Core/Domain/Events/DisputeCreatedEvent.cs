using MediatR;

namespace TrafficCourts.Domain.Events;

/// <summary>
/// Rasied when a dispute is created.
/// </summary>
/// <param name="id"></param>
public record DisputeCreatedEvent(long Id) : INotification;

