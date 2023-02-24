namespace TrafficCourts.Coms.Client;

public static class FileProperty
{
    [Obsolete($"Use {nameof(TcoDisputeId)} or {nameof(OccamDisputeId)}")]
    public const string TicketNumber = "ticket-number";

    /// <summary>
    /// The TCO dispute id.
    /// </summary>
    public const string TcoDisputeId = "tco-dispute-id";

    /// <summary>
    /// The OCCAM dispute id.
    /// </summary>
    public const string OccamDisputeId = "occam-dispute-id";

    /// <summary>
    /// 
    /// </summary>
    public const string NoticeOfDisputeId = "notice-of-dispute-id";

    /// <summary>
    /// The virus scan status.
    /// </summary>    
    public const string VirusScanStatus = "virus-scan-status";

    /// <summary>
    /// The detected virus name
    /// </summary>
    public const string VirusName = "virus-name";
}
