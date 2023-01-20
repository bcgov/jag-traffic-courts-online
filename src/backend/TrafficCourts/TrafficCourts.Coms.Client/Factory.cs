namespace TrafficCourts.Coms.Client;

internal static class Factory
{
    /// <summary>
    /// Creates the Tags dictionary from the source with the correct <see cref="IEqualityComparer"/>.
    /// </summary>
    public static Dictionary<string, string> CreateTags(IEnumerable<KeyValuePair<string, string>>? source = null)
    {
        if (source is null)
        {
            return new Dictionary<string, string>(StringComparer.Ordinal);
        }

        return new Dictionary<string, string>(source, StringComparer.Ordinal);
    }
}
