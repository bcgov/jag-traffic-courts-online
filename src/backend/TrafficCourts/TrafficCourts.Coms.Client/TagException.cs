namespace TrafficCourts.Coms.Client;

/// <summary>
/// Base class for tag related exceptions.
/// </summary>
public abstract class TagException : ObjectManagementServiceException
{
    protected TagException(string message) : base(message) { }
}
