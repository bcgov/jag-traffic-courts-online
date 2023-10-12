using Microsoft.AspNetCore.Http;

namespace TrafficCourts.Diagnostics
{
    /// <summary>
    /// Provides common endpoint filtering. 
    /// </summary>
    /// <remarks>
    /// This class should really be in the TrafficCourts.Core project, but due to references
    /// on HttpContext, it is problematic adding the correct references.
    /// </remarks>
    public static class PrometheusScraping
    {
        private const int DefaultLocalPort = 9090;

        /// <summary>
        /// Determines if metrics should be exposed on the context.
        /// </summary>
        public static bool EndpointFilter(HttpContext context)
        {
            var port = DefaultLocalPort;

            // could dynamically find the last configured url for metrics, 
            // but lets just use the same one dotnet-monitor does by default

            //var env = Environment.GetEnvironmentVariable("ASPNETCORE_URLS");
            //if (!string.IsNullOrEmpty(env))
            //{
            //    var urls = env.Split(';');
            //}

            return context.Request.Path == "/metrics" && context.Connection.LocalPort == port;
        }
    }
}
