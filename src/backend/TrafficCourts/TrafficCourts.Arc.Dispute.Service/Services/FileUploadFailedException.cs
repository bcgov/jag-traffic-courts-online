using System.Runtime.Serialization;

namespace TrafficCourts.Arc.Dispute.Service.Services;

[Serializable]
public class FileUploadFailedException : Exception
{
    public FileUploadFailedException(string? message, Exception? innerException) : base(message, innerException) { }
}
