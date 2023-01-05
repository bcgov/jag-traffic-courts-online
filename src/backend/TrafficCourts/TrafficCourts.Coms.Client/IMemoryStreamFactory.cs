namespace TrafficCourts.Coms.Client;

/// <summary>
/// Provides a method to create a new memory stream.
/// </summary>
public interface IMemoryStreamFactory
{
    MemoryStream GetStream();
}
