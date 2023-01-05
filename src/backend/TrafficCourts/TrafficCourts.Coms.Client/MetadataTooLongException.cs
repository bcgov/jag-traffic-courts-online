namespace TrafficCourts.Coms.Client;

/// <summary>
/// The exception that is thrown when the metadata is too long.
/// </summary>
public class MetadataTooLongException : MetadataException
{
    public MetadataTooLongException() : base("Total metadata length is bigger than 2048")
    {
    }
}
