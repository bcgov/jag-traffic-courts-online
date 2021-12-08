namespace TrafficCourts.Common.Health
{
    public static class HealthCheckType
    {
        /// <summary>
        /// Represents a "liveness" check. A service that fails a liveness check is considered to be unrecoverable and has to be restarted by the orchestrator.
        /// </summary>
        public const string Liveness = "liveness";

        /// <summary>
        /// Represents a "readiness" check. A service that fails a readiness check is considered to be unable to serve traffic temporarily.
        /// The orchestrator doesn't restart a service that fails this check, but stops sending traffic to it until it responds to this check positively again.
        /// </summary>
        public const string Readiness = "readiness";
    }
}
