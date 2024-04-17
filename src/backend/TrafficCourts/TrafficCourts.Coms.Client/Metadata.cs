namespace TrafficCourts.Coms.Client;

internal static class Metadata
{
    // metadata keys - used when fetching just metadata about files
    private const string Id = "coms-id";
    private const string Name = "coms-name";

    // metadata header keys - used when we get the object
    private const string IdHeader = "x-amz-meta-coms-id";
    private const string NameHeader = "x-amz-meta-coms-name";

    private static readonly StringComparer _comparer= StringComparer.OrdinalIgnoreCase;

    public static bool IsInternal(string key)
    {
        return _comparer.Equals(key, Id) || _comparer.Equals(key, Name);
    }

    public static bool IsNotInternal(KeyValuePair<string, string> item) => !IsInternal(item.Key);
    public static bool IsInternal(KeyValuePair<string, string> item) => IsInternal(item.Key);
    public static bool IsName(string key) => _comparer.Equals(key, Name);

    /// <summary>
    /// Creates the Metadata dictionary from the source with the correct <see cref="IEqualityComparer"/>.
    /// </summary>
    public static IReadOnlyDictionary<string, string> Create(IEnumerable<KeyValuePair<string, string>>? source = null)
    {
        if (source is null)
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        return new Dictionary<string, string>(source.Where(IsNotInternal), StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Gets the filename from the headers.
    /// </summary>
    /// <param name="headers"></param>
    /// <returns></returns>
    public static string? GetFilename(this IReadOnlyDictionary<string, IEnumerable<string>> headers)
    {
        if (headers is not null && headers.TryGetValue(NameHeader, out var values))
        {
            return values.FirstOrDefault();
        }

        return null;
    }
}
