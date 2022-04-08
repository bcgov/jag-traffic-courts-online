using System.Diagnostics;

namespace TrafficCourts.Staff.Service.Logging;

public static class Diagnostics
{
    public const string ServiceName = "staff-api";
    public static readonly ActivitySource Source = new ActivitySource(ServiceName);
}
