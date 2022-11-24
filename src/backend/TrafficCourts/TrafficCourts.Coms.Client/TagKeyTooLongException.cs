namespace TrafficCourts.Coms.Client;


/// <summary>
/// The exception that is thrown when the tag key is too long. A tag key can only be up to 128 characters long.
/// </summary>
public class TagKeyTooLongException : TagException
{
    public string Key { get; private set; }

    public TagKeyTooLongException(string key) : base("Tag key too long. Tag keys can only be up to 128 characters long.") 
    {
        Key = key ?? string.Empty;
    }
}
