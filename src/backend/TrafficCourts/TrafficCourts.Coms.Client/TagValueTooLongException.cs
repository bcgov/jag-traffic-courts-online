namespace TrafficCourts.Coms.Client;


/// <summary>
/// The exception that is thrown when the value of a tag is too long. A tag value can only be up to 256 characters long.
/// </summary>
public class TagValueTooLongException : Exception
{
    public string Key { get; private set; }
    public string Value { get; private set; }

    public TagValueTooLongException(string key, string value) : base("Tag value too long. Tag values can only be up to 256 characters long.")
    {
        Key = key ?? string.Empty;
        Value = value ?? string.Empty;
    }
}
