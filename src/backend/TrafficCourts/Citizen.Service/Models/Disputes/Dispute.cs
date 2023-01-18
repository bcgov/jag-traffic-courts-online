using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;

namespace TrafficCourts.Citizen.Service.Models.Disputes;

/// <summary>
/// Represents a violation ticket notice of dispute.
/// </summary>
public class Dispute : DisputantContactInformation
{
    /// <summary>
    /// The violation ticket number.
    /// </summary>
    [JsonPropertyName("ticket_number")]
    [MaxLength(12)]
    public string? TicketNumber { get; set; } = null!;

    /// <summary>
    /// The disputant's birthdate.
    /// </summary>
    [JsonPropertyName("disputant_birthdate")]
    [SwaggerSchema(Format = "date")]
    public DateTime DisputantBirthdate { get; set; }

    /// <summary>
    /// The drivers licence number. Note not all jurisdictions will use numeric drivers licence numbers.
    /// </summary>
    [JsonPropertyName("drivers_licence_number")]
    public string DriversLicenceNumber { get; set; } = null!;

    /// <summary>
    /// The province or state the drivers licence was issued by.
    /// </summary>
    [JsonPropertyName("drivers_licence_province")]
    public string DriversLicenceProvince { get; set; } = null!;

    /// <summary>
    /// The province sequence number of the drivers licence was issued by.
    /// </summary>
    [JsonPropertyName("drivers_licence_province_seq_no")]
    public int? DriversLicenceProvinceSeqNo { get; set; }

    /// <summary>
    /// The country code of the drivers licence was issued by.
    /// </summary>
    [JsonPropertyName("drivers_licence_country_id")]
    public int? DriversLicenceCountryId { get; set; }

    /// <summary>
    /// The disputant's work phone number.
    /// </summary>
    [JsonPropertyName("work_phone_number")]
    public string? WorkPhoneNumber { get; set; }

    /// <summary>
    /// The disputant intends to be represented by a lawyer at the hearing.
    /// </summary>
    [JsonPropertyName("represented_by_lawyer")]
    public DisputeRepresentedByLawyer RepresentedByLawyer { get; set; } = DisputeRepresentedByLawyer.N;

    /// <summary>
    /// Name of the law firm that will represent the disputant at the hearing.
    /// </summary>
    [JsonPropertyName("law_firm_name")]
    public string? LawFirmName { get; set; } = String.Empty;

    /// <summary>
    /// Surname of the lawyer who will represent the disputant at the hearing.
    /// </summary>
    [JsonPropertyName("lawyer_surname")]
    public string? LawyerSurname { get; set; } = String.Empty;

    /// <summary>
    /// Given Name 1 of the lawyer who will represent the disputant at the hearing.
    /// </summary>
    [JsonPropertyName("lawyer_given_name1")]
    public string? LawyerGivenName1 { get; set; } = String.Empty;

    /// <summary>
    /// Given Name 2 of the lawyer who will represent the disputant at the hearing.
    /// </summary>
    [JsonPropertyName("lawyer_given_name2")]
    public string? LawyerGivenName2 { get; set; } = String.Empty;

    /// <summary>
    /// Given Name 3 of the lawyer who will represent the disputant at the hearing.
    /// </summary>
    [JsonPropertyName("lawyer_given_name3")]
    public string? LawyerGivenName3 { get; set; } = String.Empty;

    /// <summary>
    /// Email address of the lawyer who will represent the disputant at the hearing.
    /// </summary>
    [JsonPropertyName("lawyer_email")]
    public string? LawyerEmail { get; set; } = String.Empty;

    /// <summary>
    /// Address of the lawyer who will represent the disputant at the hearing.
    /// </summary>
    [JsonPropertyName("lawyer_address")]
    public string? LawyerAddress { get; set; } = String.Empty;

    /// <summary>
    /// Address of the lawyer who will represent the disputant at the hearing.
    /// </summary>
    [JsonPropertyName("lawyer_phone_number")]
    public string? LawyerPhoneNumber { get; set; } = String.Empty;

    /// <summary>
    /// The disputant requires spoken language interpreter. The language name is indicated in this field.
    /// </summary>
    [JsonPropertyName("interpreter_language_cd")]
    public string? InterpreterLanguageCd { get; set; }

    /// <summary>
    /// Interpreter Required
    /// </summary>
    [JsonPropertyName("interprer_required")]
    public DisputeInterpreterRequired? InterpreterRequired { get; set; } = DisputeInterpreterRequired.N;

    /// <summary>
    /// The number of witnesses that the disputant intends to call.
    /// </summary>
    [JsonPropertyName("witness_no")]
    public int WitnessNo { get; set; }

    /// <summary>
    /// The reason that disputant declares for requesting a fine reduction.
    /// </summary>
    [JsonPropertyName("fine_reduction_reason")]
    public string? FineReductionReason { get; set; }

    /// <summary>
    /// The reason that disputant declares for requesting more time to pay the amount on the violation ticket.
    /// </summary>
    [JsonPropertyName("time_to_pay_reason")]
    public string? TimeToPayReason { get; set; }

    /// <summary>
    /// Dispute Counts
    /// </summary>
    [JsonPropertyName("dispute_counts")]
    public ICollection<DisputeCount>? DisputeCounts { get; set; }
}
