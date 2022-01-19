namespace TrafficCourts.Ticket.Search.Service.OpenShift
{
    public static class PlatformEnvironment
    {
        public static bool IsOpenShift => OpenShiftEnvironment.IsOpenShift;
    }
}
