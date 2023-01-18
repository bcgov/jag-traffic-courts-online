using System.Text.Json.Serialization;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using Newtonsoft.Json;

namespace TrafficCourts.Staff.Service.Models;

/// <summary>
/// Represents a violation ticket notice of dispute.
/// </summary>
public class DisputeWithUpdates
{

    /// <summary>
    /// ID
    /// </summary>
    [JsonPropertyName("disputeId")]
    public long DisputeId { get; set; }

    /// <summary>
    /// Ticket Number
    /// </summary>
    [JsonPropertyName("ticketNumber")]
    public string TicketNumber { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp that disputant submitted the notice of dispute
    /// </summary>
    [JsonPropertyName("submittedTs")]
    public System.DateTimeOffset? SubmittedTs { get; set; }

    /// <summary>
    /// Disputant Surname
    /// </summary>
    [JsonPropertyName("disputantSurname")]
    public string DisputantSurname { get; set; } = string.Empty;

    /// <summary>
    /// Disputant Given Name 1
    /// </summary>
    [JsonPropertyName("disputantGivenName1")]
    public string DisputantGivenName1 { get; set; } = string.Empty;

    /// <summary>
    /// Disputant Given Name 2
    /// </summary>
    [JsonPropertyName("disputantGivenName2")]
    public string DisputantGivenName2 { get; set; } = string.Empty;

    /// <summary>
    /// Disputant Given Name 3
    /// </summary>
    [JsonPropertyName("disputantGivenName3")]
    public string DisputantGivenName3 { get; set; } = string.Empty;

    /// <summary>
    /// Dispute status
    /// </summary>
    [JsonPropertyName("status")]
    public DisputeStatus Status { get; set; }

    /// <summary>
    /// VTC Staff assigned to dispute
    /// </summary>
    [JsonPropertyName("userAssignedTo")]
    public string UserAssignedTo { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp that VTC Staff was assigned to dispute
    /// </summary>
    [JsonPropertyName("userAssignedTs")]
    public System.DateTimeOffset? UserAssignedTs { get; set; }

    /// <summary>
    /// Whether or Not there is a pending request for an adjournment from the Disputant
    /// </summary>
    [JsonPropertyName("adjournmentDocument")]
    public bool? AdjournmentDocument { get; set; } = null!;

    /// <summary>
    /// Whether or not there is a pending request for a change of Plea from the Disputant
    /// </summary>
    [JsonPropertyName("changeOfPlea")]
    public bool? ChangeOfPlea { get; set; }

    /// <summary>
    /// Date of next upcoming hearing (if there is one)
    /// </summary>
    [JsonPropertyName("hearingDate")]
    public DateTimeOffset? HearingDate { get; set; }

    /// <summary>
    /// Email address
    /// </summary>
    [JsonPropertyName("emailAddress")]
    public string? EmailAddress { get; set; }

    /// <summary>
    /// Email address verified
    /// </summary>
    [JsonPropertyName("emailAddressVerified")]
    public bool? EmailAddressVerified { get; set; }
}

public class DocumentUpdateJSON
{
    public string? DocumentId { get; set; }
    public string? DocumentType { get; set; }
}

public class CountUpdateJSON
{
    public DisputeCountPleaCode? pleaCode { get; set; }
}
