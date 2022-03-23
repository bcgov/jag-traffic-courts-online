using Minio.Exceptions;

namespace TrafficCourts.Common.Features.FilePersistence;

public class MinioFilePersistenceException : FilePersistenceException
{
    public MinioFilePersistenceException(string message, MinioException innerException) : base(message, innerException)
    {
    }
}



