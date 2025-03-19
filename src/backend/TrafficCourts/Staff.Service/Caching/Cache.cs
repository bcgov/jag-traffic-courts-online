namespace TrafficCourts.Staff.Service.Caching;

public class Cache
{
    /// <summary>
    /// The prefix of items cached by the staff api
    /// </summary>
    public const string Prefix = "staff:";

    /// <summary>
    /// Keycloak named cache
    /// </summary>
    public static class Keycloak
    {
        /// <summary>
        /// Stores the ICollection&lt;GroupRepresentation&gt; by group name.
        /// </summary>
        /// <param name="group">The group</param>
        /// <param name="version">The version of the data structure. If new data structure is used, defined a new version. Defaults to 1.</param>
        /// <returns></returns>
        public static string UsersByGroup(string group, int version = 1) => $"keycloak:v{version}:groups:{group}";

        /// <summary>
        /// Stores the ICollection&lt;GroupRepresentation&gt; by user name.
        /// </summary>
        /// <param name="user">The user</param>
        /// <param name="version">The version of the data structure. If new data structure is used, defined a new version. Defaults to 1.</param>
        /// <returns></returns>
        public static string UsersByUsername(string user, int version = 1) => $"keycloak:v{version}:users:{user}";
    }

    /// <summary>
    /// Oracle data named cache. Stores data cached queried from Oracle.
    /// </summary>
    /// <remarks>
    /// Stored in the shared library because we may need to clear cached data on
    /// changes to disputes.
    /// </remarks>
    public static class OracleData
    {
        /// <summary>
        /// Stores the ICollection&lt;DisputeListItem&gt; items fetched from GetAllDisputes
        /// </summary>
        /// <param name="version">The version of the data structure. If new data structure is used, defined a new version. Defaults to 1.</param>
        public static string DisputeListItems(int version = 1) => $"oracle:v{version}:all:dispute-list-items";
    }
}
