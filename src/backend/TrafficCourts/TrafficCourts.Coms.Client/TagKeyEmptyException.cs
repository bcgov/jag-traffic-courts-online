namespace TrafficCourts.Coms.Client;

/// <summary>
/// The exception that is thrown when the a tag key is null or empty.
/// </summary>
public class TagKeyEmptyException : TagException
{
    public string Key { get; private set; }

    public TagKeyEmptyException(string key) : base("Tag key is empty.")
    {
        Key = key;
    }
}