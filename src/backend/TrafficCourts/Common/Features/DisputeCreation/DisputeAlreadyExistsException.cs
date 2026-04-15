namespace TrafficCourts.Common.Features.DisputeCreation;

public class DisputeAlreadyExistsException : Exception
{
    public DisputeAlreadyExistsException() { }

    public DisputeAlreadyExistsException(string message) : base(message) { }

    public DisputeAlreadyExistsException(string message, Exception inner) : base(message, inner) { }
}
