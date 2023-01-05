namespace TrafficCourts.Coms.Client;

internal static class Factory
{
    /// <summary>
    /// Creates the Metadata dictionary from the source with the correct <see cref="IEqualityComparer"/>.
    /// </summary>
    public static Dictionary<string, string> CreateMetadata(IDictionary<string, string>? source = null)
    {
        if (source is null)
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        return new Dictionary<string, string>(source, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Creates the Tags dictionary from the source with the correct <see cref="IEqualityComparer"/>.
    /// </summary>
    public static Dictionary<string, string> CreateTags(IDictionary<string, string>? source = null)
    {
        if (source is null)
        {
            return new Dictionary<string, string>(StringComparer.Ordinal);
        }

        return new Dictionary<string, string>(source, StringComparer.Ordinal);
    }
}
