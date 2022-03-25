using System.Diagnostics;

namespace TrafficCourts.Citizen.Service.Logging
{
    public static class Diagnostics
    {
        public static readonly ActivitySource Source = new ActivitySource("Citizen-API");
    }
}
