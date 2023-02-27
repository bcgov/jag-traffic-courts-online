namespace TrafficCourts.Common.Models;

/// <summary>
/// Base class for file properties.
/// </summary>
public abstract class FileProperties
{
    private static class PropertyName
    {
        public const string Internal = "internal";
    }

    protected FileProperties()
    {
    }

    protected FileProperties(IReadOnlyDictionary<string, string> metadata, IReadOnlyDictionary<string, string> tags)
    {
        ArgumentNullException.ThrowIfNull(metadata);
        ArgumentNullException.ThrowIfNull(tags);

        Internal = GetBooleanProperty(PropertyName.Internal, metadata) ?? false;
    }

    /// <summary>
    /// Determines if the file is an internal file, like OCR results, that are not shown
    /// in user lists. Defaults to false.
    /// </summary>
    public bool Internal { get; set; }

    /// <summary>
    /// Creates the metadata properties.
    /// </summary>
    /// <returns></returns>
    public IReadOnlyDictionary<string, string> ToMetadata()
    {
        Dictionary<string, string> properties = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        SetMetadata(properties);
        return properties;
    }

    public IReadOnlyDictionary<string, string> ToTags()
    {
        Dictionary<string, string> properties = new Dictionary<string, string>();

        SetTags(properties);

        // validdate

        // you can associate up to 10 tags with an object. Tags associated with an object must have unique tag keys.
        if (properties.Count > 10) throw new InvalidOperationException("Too many properties, only 10 properties can be specified");

        // a tag key can be up to 128 Unicode characters in length
        if (properties.Keys.Any(key => key.Length > 128)) throw new InvalidOperationException("Key is too long");

        // tag values can be up to 256 Unicode characters in length (COMS limits to 255)
        if (properties.Values.Any(value => value.Length > 255)) throw new InvalidOperationException("Value is too long");

        return properties;
    }

    protected virtual void SetTags(Dictionary<string, string> properties)
    {
    }

    protected virtual void SetMetadata(Dictionary<string, string> properties)
    {
        properties.Add(PropertyName.Internal, Internal ? "true" : "false");
    }

    protected static string? GetStringProperty(string name, IReadOnlyDictionary<string, string> properties)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(properties);

        properties.TryGetValue(name, out string? value);

        if (string.IsNullOrWhiteSpace(value))
        {
            value = null;
        }

        return value;
    }

    protected static long? GetInt64Property(string name, IReadOnlyDictionary<string, string> properties)
    {
        string? stringValue = GetStringProperty(name, properties);
        if (stringValue is not null)
        {
            if (long.TryParse(stringValue, out long value))
            {
                return value;
            }
        }

        return null;
    }

    protected static Guid? GetGuidProperty(string name, IReadOnlyDictionary<string, string> properties)
    {
        string? stringValue = GetStringProperty(name, properties);
        if (stringValue is not null)
        {
            if (Guid.TryParse(stringValue, out Guid value))
            {
                return value;
            }
        }

        return null;
    }

    protected static bool? GetBooleanProperty(string name, IReadOnlyDictionary<string, string> properties)
    {
        string? stringValue = GetStringProperty(name, properties);
        if (stringValue is not null)
        {
            if (bool.TryParse(stringValue, out bool value))
            {
                return value;
            }
        }

        return null;
    }

    protected static TEnum? GetEnumProperty<TEnum>(string name, IReadOnlyDictionary<string, string> properties)
        where TEnum : struct
    {
        string? stringValue = GetStringProperty(name, properties);

        if (stringValue is not null)
        {
            if (Enum.TryParse(stringValue, out TEnum value))
            {
                return value;
            }
        }

        return null;
    }
}
