using MediatR;

namespace TrafficCourts.Domain.Events;

/// <summary>
/// Raised when the disputes are unassigned.
/// </summary>
public record DisputesUnassignedEvent : INotification;
