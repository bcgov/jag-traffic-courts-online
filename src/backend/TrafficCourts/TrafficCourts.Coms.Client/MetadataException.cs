namespace TrafficCourts.Coms.Client;

public abstract class MetadataException : ObjectManagementServiceException
{
    protected MetadataException(string message) : base(message) { }
}
