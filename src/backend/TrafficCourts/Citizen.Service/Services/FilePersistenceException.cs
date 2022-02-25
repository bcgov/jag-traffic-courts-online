namespace TrafficCourts.Citizen.Service.Services
{
    public abstract class FilePersistenceException : Exception
    {
        protected FilePersistenceException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }



}
