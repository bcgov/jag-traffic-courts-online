using System.Diagnostics;

namespace TrafficCourts.Coms.Client.Monitoring;

internal static class Diagnostics
{
    public static readonly ActivitySource Source = new ActivitySource("coms");
}
