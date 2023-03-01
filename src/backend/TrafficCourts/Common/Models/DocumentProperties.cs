using TrafficCourts.Common.OpenAPIs.VirusScan.V1;

namespace TrafficCourts.Common.Models;

/// <summary>
/// Contains the common document properties across the staff and citizen portal applications.
/// </summary>
public class DocumentProperties : FileProperties
{
    private static class PropertyName
    {
        public const string VirusScanStatus = "virus-scan-status";
        public const string VirusName = "virus-name";
        public const string TcoDisputeId = "tco-dispute-id";
        public const string OccamDisputeId = "occam-dispute-id";
        public const string NoticeOfDisputeId = "notice-of-dispute-id";
        public const string DocumentType = "document-type";
        public const string DocumentSource = "document-source";
        public const string StaffReviewStatus = "staff-review-status";
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
        DocumentSource = GetEnumProperty<DocumentSource>(PropertyName.DocumentSource, metadata);
        DocumentType = GetStringProperty(PropertyName.DocumentType, tags);

        // virus scan properties
        _virusScanStatus = GetStringProperty(PropertyName.VirusScanStatus, tags);
        VirusName = GetStringProperty(PropertyName.VirusName, tags);

        StaffReviewStatus = GetStringProperty(PropertyName.StaffReviewStatus, tags);

        // Staff Portal properties
        TcoDisputeId = GetInt64Property(PropertyName.TcoDisputeId, metadata);

        // Citizen Portal properties
        NoticeOfDisputeId = GetGuidProperty(PropertyName.NoticeOfDisputeId, metadata);
    }

    protected override void SetTags(Dictionary<string, string> properties)
    {
        // Common properties
        if (!string.IsNullOrEmpty(DocumentType)) properties.Add(PropertyName.DocumentType, DocumentType);

        // add virus scan properties
        if (!string.IsNullOrEmpty(_virusScanStatus)) properties.Add(PropertyName.VirusScanStatus, _virusScanStatus);
        if (!string.IsNullOrEmpty(VirusName)) properties.Add(PropertyName.VirusName, VirusName);

        if (!string.IsNullOrEmpty(StaffReviewStatus)) properties.Add(PropertyName.StaffReviewStatus, StaffReviewStatus);
    }

    protected override void SetMetadata(Dictionary<string, string> properties)
    {
        base.SetMetadata(properties);

        if (DocumentSource is not null) properties.Add(PropertyName.DocumentSource, DocumentSource.Value.ToString());

        // Staff Portal properties
        if (TcoDisputeId is not null) properties.Add(PropertyName.TcoDisputeId, TcoDisputeId.Value.ToString());

        // Citizen Portal properties
        if (NoticeOfDisputeId is not null) properties.Add(PropertyName.NoticeOfDisputeId, NoticeOfDisputeId.Value.ToString("d"));
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
