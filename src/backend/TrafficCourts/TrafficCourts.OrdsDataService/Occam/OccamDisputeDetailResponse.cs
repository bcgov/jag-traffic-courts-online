using System.Text.Json.Serialization;

namespace TrafficCourts.OrdsDataService.Occam;

/// <summary>
/// Represents the response for the /v2/occam_dispute?dispute_id={dispute_id}&violation_ticket_upload_id={violation_ticket_upload_id} endpoint.
/// </summary>
public class OccamDisputeDetailResponse
{
    [JsonPropertyName("occam_violation_ticket_uploads")]
    public IList<OccamViolationTicketUploads>? OccamViolationTicketUploads { get; set; }

    [JsonPropertyName("occam_violation_ticket_counts")]
    public IList<OccamViolationTicketCounts>? OccamViolationTicketCounts { get; set; }

    [JsonPropertyName("occam_disputes")]
    public IList<OccamDisputes>? OccamDisputes { get; set; }

    [JsonPropertyName("occam_dispute_counts")]
    public IList<OccamDisputeCounts>? OccamDisputeCounts { get; set; }

    [JsonPropertyName("occam_audit_log_entries")]
    public IList<OccamAuditLogEntries>? OccamAuditLogEntries { get; set; }

    [JsonPropertyName("occam_dispute_update_requests")]
    public IList<OccamDisputeUpdateRequests>? OccamDisputeUpdateRequests { get; set; }

    [JsonPropertyName("occam_outgoing_emails")]
    public IList<OccamOutgoingEmails>? OccamOutgoingEmails { get; set; }

    [JsonPropertyName("errors")]
    public IList<OrdsDataServiceError>? Errors { get; set; }

    /// <summary>
    /// Gets the list of <see cref="OccamViolationTicketUploads"/> with their associated disputes and counts.
    /// </summary>
    public IEnumerable<OccamViolationTicketUploads> GetViolationTicketUploads()
    {
        Clean();

        EnsureCollectionsInitialized();

        foreach (var ticketUpload in OccamViolationTicketUploads!)
        {
            Process(ticketUpload); // populate the ticket upload with the dispute and counts
            yield return ticketUpload;
        }
    }

    private void Process(OccamViolationTicketUploads? ticketUpload)
    {
        if (ticketUpload is null)
        {
            return;
        }

        // find the dispute associated with this ticket upload
        OccamDisputes? dispute = GetDispute(ticketUpload);

        if (dispute is not null)
        {
            ticketUpload.Dispute = dispute;

            // create the reverse navigation
            dispute.ViolationTicketUpload = ticketUpload;
            dispute.AuditLogEntries.AddRange(GetAuditLogEntries(dispute));
            dispute.DisputeUpdateRequests.AddRange(GetDisputeUpdateRequests(dispute));
            dispute.OutgoingEmails.AddRange(GetOutgoingEmailsForDispute(dispute));
        }

        foreach (var ticketCount in GetTicketCounts(ticketUpload))
        {
            ticketCount.Dispute = GetDisputeCount(ticketCount);
            ticketUpload.Counts.Add(ticketCount);
        }
    }

    /// <summary>
    /// Gets the <see cref="OccamDisputes"/> matching the supplied ticket upload.
    /// </summary>
    private OccamDisputes? GetDispute(OccamViolationTicketUploads ticketUpload)
    {
        return OccamDisputes?.SingleOrDefault(dispute => dispute.ViolationTicketUploadId == ticketUpload.ViolationTicketUploadId);
    }

    /// <summary>
    /// Gets the <see cref="OccamDisputeCounts"/> matching the supplied ticket count.
    /// </summary>
    private OccamDisputeCounts? GetDisputeCount(OccamViolationTicketCounts ticketCount)
    {
        return OccamDisputeCounts?.SingleOrDefault(disputeCount => disputeCount.ViolationTicketCountId == ticketCount.ViolationTicketCountId);
    }

    /// <summary>
    /// Gets the list of <see cref="OccamViolationTicketCounts"/> matching the supplied ticket upload.
    /// </summary>
    private IEnumerable<OccamViolationTicketCounts> GetTicketCounts(OccamViolationTicketUploads ticketUpload)
    {
        return OccamViolationTicketCounts?
            .Where(ticketCount => ticketCount.ViolationTicketCountId == ticketUpload.ViolationTicketUploadId)
            .OrderBy(ticketCount => ticketCount.CountNo) ?? Enumerable.Empty<OccamViolationTicketCounts>();
    }

    /// <summary>
    /// Gets the list of <see cref="OccamAuditLogEntries"/> matching the supplied dispute.
    /// </summary>
    private IEnumerable<OccamAuditLogEntries> GetAuditLogEntries(OccamDisputes dispute)
    {
        return OccamAuditLogEntries?.Where(auditLogEntry => auditLogEntry.DisputeId == dispute.DisputeId) ?? [];
    }

    /// <summary>
    /// Gets the list of <see cref="OccamDisputeUpdateRequests"/> matching the supplied dispute.
    /// </summary>
    private IEnumerable<OccamDisputeUpdateRequests> GetDisputeUpdateRequests(OccamDisputes dispute)
    {
        return OccamDisputeUpdateRequests?.Where(updateRequest => updateRequest.DisputeId == dispute.DisputeId) ?? [];
    }

    /// <summary>
    /// Gets the list of <see cref="OccamOutgoingEmails"/> matching the supplied dispute.
    /// </summary>
    private IEnumerable<OccamOutgoingEmails> GetOutgoingEmailsForDispute(OccamDisputes dispute)
    {
        return OccamOutgoingEmails?.Where(outgoingEmail => outgoingEmail.DisputeId == dispute.DisputeId) ?? [];
    }


    /// <summary>
    /// Ensures that all data collections are initialized to avoid null reference exceptions.
    /// </summary>
    private void EnsureCollectionsInitialized()
    {
        OccamViolationTicketUploads ??= [];
        OccamViolationTicketCounts ??= [];
        OccamDisputes ??= [];
        OccamDisputeCounts ??= [];
        OccamAuditLogEntries ??= [];
        OccamDisputeUpdateRequests ??= [];
        OccamOutgoingEmails ??= [];
    }

    /// <summary>
    /// Resets all the dirty flags to <c>false</c> on the collections.
    /// </summary>
    private void Clean()
    {
        // reset all the fields not dirty
        OccamViolationTicketUploads?.SetDirty(false);
        OccamViolationTicketCounts?.SetDirty(false);
        OccamDisputes?.SetDirty(false);
        OccamDisputeCounts?.SetDirty(false);
        OccamAuditLogEntries?.SetDirty(false);
        OccamDisputeUpdateRequests?.SetDirty(false);
        OccamOutgoingEmails?.SetDirty(false);
    }
}


