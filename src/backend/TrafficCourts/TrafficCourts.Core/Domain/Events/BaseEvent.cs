using MediatR;

namespace TrafficCourts.Domain.Events;

/// <summary>
/// Base event for all notifications
/// </summary>
public abstract class BaseEvent : INotification
{
}
