using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TrafficCourts.Citizen.Service.Models.Tickets;

/// <summary>
/// Represents a violation ticket count.
/// </summary>
public class ViolationTicketCount
{
    /// <summary>
    /// The count number. Must be unique within an individual violation ticket.
    /// </summary>
    [JsonPropertyName("count")]
    [Range(1, 3)]
    public short Count { get; set; }

    /// <summary>
    /// The description of the offence.
    /// </summary>
    [JsonPropertyName("description")]
    [MaxLength(50)]
    public string? Description { get; set; }

    /// <summary>
    /// The act or regulation code the violation occurred against. For example, MVA, WLA, TCR, etc
    /// </summary>
    [JsonPropertyName("act_or_regulation")]
    [MaxLength(3)]
    public string? ActRegulation { get; set; }

    /// <summary>
    /// The full section designation of the act or regulation. For example, "147(1)" which means "Speed in school zone"
    /// </summary>
    [JsonPropertyName("section")]
    [MaxLength(20)]
    public string? Section { get; set; }

    /// <summary>
    /// The ticketed amount.
    /// </summary>
    /// <remarks>The upper bound arbitrarily set to just under one million.</remarks>
    [JsonPropertyName("ticketed_amount")]
    [Range(1f, 999_999f)]
    public decimal? TicketedAmount { get; set; }

    /// <summary>
    /// The amount due. Will be the same as ticketed_amount if no payments have been made on the ticket. May or may not have discount computed.
    /// </summary>
    /// <remarks>This field is for display purposes only. If not set, it would default to the <see cref="TicketedAmount"/></remarks>
    /// <remarks>The upper bound arbitrarily set to just under one million.</remarks>
    [JsonPropertyName("amount_due")]
    [Range(1f, 999_999f)]
    public decimal? AmountDue { get; set; }

    /// <summary>
    /// The count is flagged as an offence to an act. Cannot be true, if is_regulation is true.
    /// </summary>
    [JsonPropertyName("is_act")]
    public bool? IsAct { get; set; }

    /// <summary>
    /// The count is flagged as an offence to a regulation. Cannot be true, if is_act is true.
    /// </summary>
    [JsonPropertyName("is_regulation")]
    public bool? IsRegulation { get; set; }
}
