using System.Diagnostics;

namespace TrafficCourts.Citizen.Service.Logging
{
    public static class Diagnostics
    {
        public const string ServiceName = "citizen-api";
        public static readonly ActivitySource Source = new ActivitySource(ServiceName);
    }
}
