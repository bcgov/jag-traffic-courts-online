using Microsoft.AspNetCore.Mvc;

namespace TrafficCourts.Common.Errors;

/// <summary>
/// A simple wrapper for a ProblemDetails return object
/// </summary>
public class HttpError : ObjectResult
{
    public HttpError(int status, string message) : base(null)
    {
        // Remove excess added to the message by the generated OpenAPI spec.
        int index = message.IndexOf("\n\nStatus");
        message = index > 0 ? message[..index] : message;

        ProblemDetails problemDetails = new ();
        problemDetails.Status = status;
        problemDetails.Extensions.Add("errors", new List<string>() { message });

        this.StatusCode = status;
        this.Value = problemDetails;
    }
}
