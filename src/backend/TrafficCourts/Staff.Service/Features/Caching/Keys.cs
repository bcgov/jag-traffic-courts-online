namespace TrafficCourts.Staff.Service.Features.Caching
{
    public static class Keys
    {
        public static class Dispute
        {
            /// <summary>
            /// Stores the ICollection&lt;DisputeListItem&gt; items fetched from GetAllDisputes
            /// </summary>
            public const string DisputeListItems = "v1:all:dispute-list-items";
        }

        public static class Keycloak
        {
            /// <summary>
            /// Stores the ICollection&lt;GroupRepresentation&gt; by group name.
            /// </summary>
            /// <param name="group">The group</param>
            /// <returns></returns>
            public static string UsersByGroup(string group) => $"v1:keycloak:groups:{group}";

            /// <summary>
            /// Stores the ICollection&lt;GroupRepresentation&gt; by user name.
            /// </summary>
            /// <param name="user">The user</param>
            /// <returns></returns>
            public static string UsersByUsername(string user) => $"v1:keycloak:users:{user}";
        }
    }
}
