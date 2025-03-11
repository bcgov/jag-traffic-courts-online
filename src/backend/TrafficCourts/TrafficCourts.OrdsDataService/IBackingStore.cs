namespace TrafficCourts.OrdsDataService;

public interface IBackingStore
{
    /// <summary>
    /// Gets a value from the backing store.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    T? Get<T>(string key);

    /// <summary>
    /// Gets a DateTime value from the backing store with the specified format.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="format">The date time format.</param>
    /// <param name="kind">The kind the returned DateTime should have.</param>
    /// <returns></returns>
    DateTime? GetDateTime(string key, string format, DateTimeKind kind = DateTimeKind.Unspecified);

    /// <summary>
    /// Set a value in the backing store
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key">The key</param>
    /// <param name="value">The value</param>
    /// <param name="alwaysDirty">
    /// If <c>true</c> the key will always be considered ditry. Primary key values should always be dirty if set.
    /// </param>
    void Set<T>(string key, T? value, bool alwaysDirty = false);

    /// <summary>
    /// Set a DateTime value in the backing store with the specified format.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="format">The date time format.</param>
    void SetDateTime(string key, DateTime? value, string format);

    /// <summary>
    /// Converts the backing store to a dictionary. Only keys that are dirty are included.
    /// </summary>
    /// <returns></returns>
    Dictionary<string, object?> ToDictionary();

    /// <summary>
    /// Determines if there are any dirty items in the backing store.
    /// </summary>
    bool Dirty { get; set; }
}
