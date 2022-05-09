using System.Diagnostics;

namespace TrafficCourts.Citizen.Service
{
    public static class Diagnostics
    {
        public static readonly ActivitySource Source = new ActivitySource("citizen-api");
    }
}
