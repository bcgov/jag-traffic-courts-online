using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TrafficCourts.Common.Models;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;

namespace TrafficCourts.Citizen.Service.Models.Disputes;

/// <summary>
/// Represents a violation ticket notice of dispute.
/// </summary>
public class NoticeOfDispute : Dispute
{
    /// <summary>
    /// Identifier for whether the citizen has detected any issues with the OCR ticket result or not.
    /// </summary>
    [JsonPropertyName("disputant_detected_ocr_issues")]
    public DisputeDisputantDetectedOcrIssues? DisputantDetectedOcrIssues { get; set; } = DisputeDisputantDetectedOcrIssues.N;

    /// <summary>
    /// The description of the issue with OCR ticket if the citizen has detected any.
    /// </summary>
    [JsonPropertyName("disputant_ocr_issues")]
    public string? DisputantOcrIssues { get; set; }

    /// <summary>
    /// The unique identifier for the Violation Ticket (OCR or looked up) for this dispute.
    /// </summary>
    [JsonPropertyName("ticket_id")]
    public string TicketId { get; set; } = null!;

    /// <summary>
    /// Detachment Location
    /// </summary>
    [JsonPropertyName("detachment_location")]
    public string? DetachmentLocation { get; set; } = null!;

    /// <summary>
    /// Violation Ticket
    /// </summary>
    [JsonPropertyName("violation_ticket")]
    public Models.Tickets.ViolationTicket? ViolationTicket { get; set; } = null!;

    /// <summary>
    /// List of file metadata that contain ID and Filename of all the uploaded documents related to this particular JJDispute
    /// </summary>
    /// 
    [JsonPropertyName("file_data")]
    public List<FileMetadata>? FileData { get; set; }
}
