namespace TrafficCourts.Messaging.MessageContracts;

/// <summary>
/// Message to indicate that a document was uploaded through COMS
/// </summary>
public class DocumentUploaded
{
    /// <summary>
    /// Id of the document uploaded
    /// </summary>
    public Guid Id { get; set; }
}
