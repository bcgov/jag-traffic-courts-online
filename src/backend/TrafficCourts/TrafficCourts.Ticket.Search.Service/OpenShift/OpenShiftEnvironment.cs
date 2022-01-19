namespace TrafficCourts.Ticket.Search.Service.OpenShift
{
    public static class OpenShiftEnvironment
    {
        private static string? _namespace;

        public static bool IsOpenShift => !string.IsNullOrWhiteSpace(GetFromEnvironmentVariable("OPENSHIFT", ref _namespace));

        private static string? GetFromEnvironmentVariable(string name, ref string? cached)
        {
            if (cached == null) cached = Environment.GetEnvironmentVariable(name) ?? string.Empty;
            return cached;
        }
    }
}
