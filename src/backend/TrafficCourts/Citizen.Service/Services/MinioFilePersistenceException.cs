using Minio.Exceptions;

namespace TrafficCourts.Citizen.Service.Services
{
    public class MinioFilePersistenceException : FilePersistenceException
    {
        public MinioFilePersistenceException(string message, MinioException innerException) : base(message, innerException)
        {
        }
    }



}
