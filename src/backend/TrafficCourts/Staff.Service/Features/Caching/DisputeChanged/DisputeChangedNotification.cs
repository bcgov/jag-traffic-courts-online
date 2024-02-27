using MediatR;

namespace TrafficCourts.Staff.Service.Features.Caching.DisputeChanged
{
    /// <summary>
    /// A dispute has change event
    /// </summary>
    public class DisputeChangedNotification : INotification
    {
        public static readonly DisputeChangedNotification Instance = new();
    }
}
