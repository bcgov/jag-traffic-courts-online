namespace TrafficCourts.Citizen.Service.Caching;

public static class Cache
{
    public static class Citizen
    {
        /// <summary>
        /// The prefix of items cached by the citizen api
        /// </summary>
        public const string Prefix = "citizen";

        /// <summary>
        /// Ticket search named cache
        /// </summary>
        public static class TicketSearch
        {
            /// <summary>
            /// Stores tickets found from the RSI service.
            /// </summary>
            public const string Name = "ticket-search";

            /// <summary>
            /// Get the cache key for searched ticket.
            /// </summary>
            /// <param name="ticketNumber">The ticket number searched for.</param>
            /// <param name="timeOnly">The time the ticket is issued.</param>
            /// <param name="version">The version of the data structure. If new data structure is used, defined a new version. Defaults to 1.</param>
            /// <returns></returns>
            public static string Key(string ticketNumber, TimeOnly timeOnly, int version = 1)
            {
                // D2 is left pad with zeros to 2 digits
                return $"v{version}:{ticketNumber}-{timeOnly.Hour:D2}:{timeOnly.Minute:D2}";
            }
        }
    }
}
