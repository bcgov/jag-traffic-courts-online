using System.Diagnostics.CodeAnalysis;

namespace TrafficCourts.Common.Features.FilePersistence;

[ExcludeFromCodeCoverage]
public abstract class FilePersistenceException : Exception
{
    protected FilePersistenceException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
