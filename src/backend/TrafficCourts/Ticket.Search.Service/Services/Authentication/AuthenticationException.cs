using System.Net;

namespace TrafficCourts.Ticket.Search.Service.Services.Authentication
{
    [Serializable]
    public class AuthenticationException : Exception
    {
        public HttpStatusCode StatusCode { get; private set; }

        public AuthenticationException(
            string? message, 
            HttpStatusCode statusCode, 
            Exception? innerException = null)
            : base(message, innerException)
        {
            StatusCode = statusCode;
        }
    }
}
