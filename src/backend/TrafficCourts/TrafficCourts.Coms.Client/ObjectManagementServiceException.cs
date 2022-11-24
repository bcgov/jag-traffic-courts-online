namespace TrafficCourts.Coms.Client;

public class ObjectManagementServiceException : Exception
{
    public ObjectManagementServiceException(string message) : base(message)
    {
    }

    public ObjectManagementServiceException(string message, Exception inner) : base(message, inner)
    {
    }
}
