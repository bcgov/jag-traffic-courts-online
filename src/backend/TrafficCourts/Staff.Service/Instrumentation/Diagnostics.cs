using System.Diagnostics;

namespace TrafficCourts.Staff.Service;

public static class Diagnostics
{
    public static readonly ActivitySource Source = new ActivitySource("staff-api");
}
