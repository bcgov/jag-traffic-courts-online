namespace TrafficCourts.Coms.Client;

internal static class Metadata
{
    // metadata keys - used when fetching just metadata about files
    internal static class Keys
    {
        public const string Id = "coms-id";
        public const string Name = "coms-name";
    }

    /// <summary>
    /// The prefix on headers when metadata is returned
    /// </summary>
    public const string HeaderPrefix = "x-amz-meta-";

    /// <summary>
    /// Creates the Metadata dictionary from the source with the correct <see cref="IEqualityComparer"/>.
    /// </summary>
    public static IReadOnlyDictionary<string, string> Create(IEnumerable<KeyValuePair<string, string>>? source = null)
    {
        if (source is null)
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        return new Dictionary<string, string>(source, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Gets the filename from the headers. Note the headers will contain the prefix "x-amz-meta-" where as 
    /// when fetching the metadata, there is no prefix.
    /// </summary>
    /// <param name="headers"></param>
    /// <returns></returns>
    public static string? GetFilename(this FileResponse response)
    {
        IReadOnlyDictionary<string, IEnumerable<string>>? headers = response?.Headers;

        if (headers is not null && headers.TryGetValue($"{HeaderPrefix}{Keys.Name}", out var values))
        {
            return values.FirstOrDefault();
        }

        return null;
    }
}
