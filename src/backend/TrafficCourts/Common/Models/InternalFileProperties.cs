namespace TrafficCourts.Common.Models;

/// <summary>
/// Interal file properties are for internal files. Internal files
/// are not displayed directly in file lists to the end users.
/// </summary>
public class InternalFileProperties : FileProperties
{
    public static class DocumentTypes
    {
        public const string TicketImage = "Ticket Image";
        public const string OcrResult = "OCR Result";
    }

    private static class PropertyName
    {
        public const string NoticeOfDisputeId = "notice-of-dispute-id";
        public const string DocumentType = "document-type";
    }

    public InternalFileProperties()
    {
        // always internal
        Internal = true;
    }

    /// <summary>
    /// The notice of dispute id.
    /// </summary>
    public Guid? NoticeOfDisputeId { get; set; }

    /// <summary>
    /// The type of document.
    /// </summary>
    public string? DocumentType { get; set; }

    protected override void SetTags(Dictionary<string, string> properties)
    {
        // Common properties
        if (!string.IsNullOrEmpty(DocumentType)) properties.Add(PropertyName.DocumentType, DocumentType);
    }

    protected override void SetMetadata(Dictionary<string, string> properties)
    {
        base.SetMetadata(properties);

        // Citizen Portal properties
        if (NoticeOfDisputeId is not null) properties.Add(PropertyName.NoticeOfDisputeId, NoticeOfDisputeId.Value.ToString("d"));
    }
}
