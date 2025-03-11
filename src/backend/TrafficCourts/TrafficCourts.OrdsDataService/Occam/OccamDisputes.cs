namespace TrafficCourts.OrdsDataService.Occam;

public partial class OccamDisputes
{
    public OccamViolationTicketUploads? ViolationTicketUpload { get; set; }
    public List<OccamAuditLogEntries> AuditLogEntries { get; } = new List<OccamAuditLogEntries>();
    public List<OccamDisputeUpdateRequests> DisputeUpdateRequests { get; } = new List<OccamDisputeUpdateRequests>();
    public List<OccamOutgoingEmails> OutgoingEmails { get; } = new List<OccamOutgoingEmails>();

}
