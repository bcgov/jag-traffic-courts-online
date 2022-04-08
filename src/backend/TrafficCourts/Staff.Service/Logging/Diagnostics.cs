using System.Diagnostics;

namespace TrafficCourts.Staff.Service.Logging;

public static class Diagnostics
{
    public static readonly ActivitySource Source = new ActivitySource("staff-api");
}
