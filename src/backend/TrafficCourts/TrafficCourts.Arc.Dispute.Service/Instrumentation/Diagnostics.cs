using System.Diagnostics;

namespace TrafficCourts.Arc.Dispute.Service;

public static class Diagnostics
{
    public static readonly ActivitySource Source = new ActivitySource("arc-dispute-api");
}
