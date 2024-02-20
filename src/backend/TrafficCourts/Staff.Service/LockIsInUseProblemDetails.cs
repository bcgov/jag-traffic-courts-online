using Microsoft.AspNetCore.Mvc;
using System.Net;
using TrafficCourts.Staff.Service.Services;

namespace TrafficCourts.Staff.Service
{
    /// <summary>
    /// Problem when a lock is already in use.
    /// </summary>
    public class LockIsInUseProblemDetails : ProblemDetails
    {
        public LockIsInUseProblemDetails(HttpContext httpContext, LockIsInUseException exception)
        {
            ArgumentNullException.ThrowIfNull(httpContext);
            ArgumentNullException.ThrowIfNull(exception);

            Status = (int)HttpStatusCode.Conflict;
            Title = exception.Source + ": Error Locking JJ Dispute";
            Instance = httpContext.Request.Path;
            string? innerExceptionMessage = exception.InnerException?.Message;

            Extensions.Add("lockedBy", exception.Username ?? string.Empty);
            if (innerExceptionMessage is not null)
            {
                Extensions.Add("errors", new string[] { exception.Message, innerExceptionMessage });
            }
            else
            {
                Extensions.Add("errors", new string[] { exception.Message });
            }
        }
    }
}
