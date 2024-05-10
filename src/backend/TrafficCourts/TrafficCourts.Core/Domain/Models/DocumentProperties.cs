using TrafficCourts.Domain.Models;

namespace TrafficCourts.Domain.Models;


/// <summary>
/// Contains the common document properties across the staff and citizen portal applications.
/// </summary>
public class DocumentProperties : FileProperties
{
    private static class PropertyName
    {
        public static class Tags
        {
            public const string DocumentType = "document-type";
            public const string OccamDisputeId = "occam-dispute-id";
            public const string StaffReviewStatus = "staff-review-status";
            public const string VirusName = "virus-name";
            public const string VirusScanStatus = "virus-scan-status";
        }

        public static class Metadata
        {
            public const string DocumentName = "coms-name";             // this one is defined in COMS
            public const string DocumentSource = "document-source";
            public const string NoticeOfDisputeId = "notice-of-dispute-id";
            public const string TcoDisputeId = "tco-dispute-id";
        }
    }

    private string? _virusScanStatus;

    public string? StaffReviewStatus { get; set; }

    /// <summary>
    /// The virus scan status. Will be either clean, infected or error.
    /// </summary>
    public string VirusScanStatus => _virusScanStatus ?? string.Empty;

    /// <summary>
    /// The detected virus name
    /// </summary>
    public string? VirusName { get; set; }

    /// <summary>
    /// The TCO / JJ dispute id.
    /// </summary>
    public long? TcoDisputeId { get; set; }

    /// <summary>
    /// The notice of dispute id.
    /// </summary>
    public Guid? NoticeOfDisputeId { get; set; }

    /// <summary>
    /// The name of document.
    /// </summary>
    public string? DocumentName { get; set; }

    /// <summary>
    /// The type of document.
    /// </summary>
    public string? DocumentType { get; set; }

    /// <summary>
    /// The source of this document, staff or citizen.
    /// </summary>
    public DocumentSource? DocumentSource { get; set; }

    /// <summary>
    /// Returns if the <see cref="VirusScanStatus"/> is not null and is set to clean.
    /// </summary>
    public bool VirusScanIsClean => StringComparer.OrdinalIgnoreCase.Equals(VirusScanStatus, "clean");

    public DocumentProperties() : base()
    {
        Internal = false;
    }

    public DocumentProperties(IReadOnlyDictionary<string, string> metadata, IReadOnlyDictionary<string, string> tags)
        : base(metadata, tags)
    {
        ArgumentNullException.ThrowIfNull(metadata);
        ArgumentNullException.ThrowIfNull(tags);

        // Common properties
        DocumentName = GetStringProperty(PropertyName.Metadata.DocumentName, metadata);
        DocumentSource = GetEnumProperty<DocumentSource>(PropertyName.Metadata.DocumentSource, metadata);
        DocumentType = GetStringProperty(PropertyName.Tags.DocumentType, tags);

        // virus scan properties
        _virusScanStatus = GetStringProperty(PropertyName.Tags.VirusScanStatus, tags);
        VirusName = GetStringProperty(PropertyName.Tags.VirusName, tags);

        StaffReviewStatus = GetStringProperty(PropertyName.Tags.StaffReviewStatus, tags);

        // Staff Portal properties
        TcoDisputeId = GetInt64Property(PropertyName.Metadata.TcoDisputeId, metadata);

        // Citizen Portal properties
        NoticeOfDisputeId = GetGuidProperty(PropertyName.Metadata.NoticeOfDisputeId, metadata);
    }

    protected override void SetTags(Dictionary<string, string> properties)
    {
        // Common properties
        if (!string.IsNullOrEmpty(DocumentType)) properties.Add(PropertyName.Tags.DocumentType, DocumentType);

        // add virus scan properties
        if (!string.IsNullOrEmpty(_virusScanStatus)) properties.Add(PropertyName.Tags.VirusScanStatus, _virusScanStatus);
        if (!string.IsNullOrEmpty(VirusName)) properties.Add(PropertyName.Tags.VirusName, VirusName);

        if (!string.IsNullOrEmpty(StaffReviewStatus)) properties.Add(PropertyName.Tags.StaffReviewStatus, StaffReviewStatus);
    }

    protected override void SetMetadata(Dictionary<string, string> properties)
    {
        base.SetMetadata(properties);

        if (DocumentName is not null) properties.Add(PropertyName.Metadata.DocumentName, DocumentName);
        if (DocumentSource is not null) properties.Add(PropertyName.Metadata.DocumentSource, DocumentSource.Value.ToString());

        // Staff Portal properties
        if (TcoDisputeId is not null) properties.Add(PropertyName.Metadata.TcoDisputeId, TcoDisputeId.Value.ToString());

        // Citizen Portal properties
        if (NoticeOfDisputeId is not null) properties.Add(PropertyName.Metadata.NoticeOfDisputeId, NoticeOfDisputeId.Value.ToString("d"));
    }

    public void SetVirusScanNotInfected()
    {
        _virusScanStatus = "clean";
        VirusName = null;
    }

    public void SetVirusScanInfected(string virusName)
    {
        ArgumentNullException.ThrowIfNull(virusName);

        _virusScanStatus = "infected";
        VirusName = virusName;
    }

    public void SetVirusScanError()
    {
        _virusScanStatus = "error";
        VirusName = null;
    }
}
