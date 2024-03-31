using MediatR;

namespace TrafficCourts.Domain.Events;

/// <summary>
/// Raised when something about a dispute is changed.
/// </summary>
/// <param name="Id">The dispute id.</param>
public record DisputeChangedEvent(long Id) : INotification;
