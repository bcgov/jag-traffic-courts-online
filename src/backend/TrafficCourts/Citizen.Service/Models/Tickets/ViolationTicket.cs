using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TrafficCourts.Common.Converters;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;

namespace TrafficCourts.Citizen.Service.Models.Tickets;

/// <summary>
/// Represents the data contained on a BC violation ticket
/// </summary>
public class ViolationTicket
{
    /// <summary>
    /// The violation ticket number.
    /// </summary>
    [JsonPropertyName("ticket_number")]
    [MaxLength(12)]
    public string? TicketNumber { get; set; }

    /// <summary>
    /// Disputant Organization Name
    /// </summary>
    [JsonPropertyName("disputant_organization_name")]
    [MaxLength(150)]
    public string? DisputantOrganizationName { get; set; }

    #region Fields related to the Issued To section
    /// <summary>
    /// The surname or corporate name.
    /// </summary>
    [JsonPropertyName("disputant_surname")]
    [MaxLength(30)]
    public string? DisputantSurname { get; set; }

    /// <summary>
    /// The given names or corporate name continued.
    /// </summary>
    [JsonPropertyName("disputant_given_names")]
    [MaxLength(100)]
    public string? DisputantGivenNames { get; set; }

    /// <summary>
    /// The person issued the ticket has been identified as a young person.
    /// </summary>
    [JsonPropertyName("is_young_person")]
    public ViolationTicketIsYoungPerson? IsYoungPerson { get; set; }

    #region Drivers Licence

    /// <summary>
    /// The drivers licence number. Note not all jurisdictions will use numeric drivers licence numbers.
    /// </summary>
    [JsonPropertyName("drivers_licence_number")]
    [MaxLength(20)]
    public string? DisputantDriversLicenceNumber { get; set; }

    /// <summary>
    /// Disputant client number
    /// </summary>
    [JsonPropertyName("disputant_client_number")]
    [MaxLength(30)]
    public string? DisputantClientNumber { get; set; }

    /// <summary>
    /// The province or state the drivers licence was issued by.
    /// </summary>
    [JsonPropertyName("drivers_licence_province")]
    [MaxLength(30)]
    public string? DriversLicenceProvince { get; set; }

    /// <summary>
    /// The year the drivers licence was produced.
    /// </summary>
    [JsonPropertyName("drivers_licence_issued_year")]
    [Range(1900, 2100)]
    public short? DriversLicenceIssuedYear { get; set; }

    /// <summary>
    /// The year the drivers licence expires.
    /// </summary>
    [JsonPropertyName("drivers_licence_expiry_year")]
    [Range(1900, 2100)]
    public short? DriversLicenceExpiryYear { get; set; }
    #endregion

    /// <summary>
    /// The birthdate of the individual the violation ticket was issued to.
    /// </summary>
    [JsonPropertyName("disputant_birthdate")]
    [SwaggerSchema(Format = "date")]
    [JsonConverter(typeof(DateOnlyJsonConverter))]
    public DateOnly? DisputantBirthdate { get; set; }

    /// <summary>
    /// The address of the individual the violation ticket was issued to.
    /// </summary>
    [JsonPropertyName("address")]
    [MaxLength(30)]
    public string? Address { get; set; }

    /// <summary>
    /// The city of the individual the violation ticket was issued to.
    /// </summary>
    [JsonPropertyName("address_city")]
    [MaxLength(30)]
    public string? AddressCity { get; set; }

    /// <summary>
    /// The province or state of the individual the violation ticket was issued to.
    /// </summary>
    [JsonPropertyName("address_province")]
    [MaxLength(30)]
    public string? AddressProvince { get; set; }

    /// <summary>
    /// The postal code or zip code.
    /// </summary>
    [JsonPropertyName("address_postal_code")]
    [MaxLength(6)]
    public string? AddressPostalCode { get; set; }

    /// <summary>
    /// The country text
    /// </summary>
    [JsonPropertyName("address_country")]
    [MaxLength(100)]
    public string? AddressCountry { get; set; }

    /// <summary>
    /// Office Pin Text
    /// </summary>
    [JsonPropertyName("officer_pin")]
    [MaxLength(10)]
    public string? OfficerPin { get; set; }

    /// <summary>
    /// Detachment Location
    /// </summary>
    [JsonPropertyName("detachment_location")]
    [MaxLength(150)]
    public string? detachmentLocation { get; set; }

    /// <summary>
    /// The address represents a change of address. The address on the violation would be different from the address 
    /// on the drivers licence or provided identification.
    /// </summary>
    [JsonPropertyName("is_change_of_address")]
    public ViolationTicketIsChangeOfAddress? IsChangeOfAddress { get; set; }

    #endregion

    #region Issued To designation
    /// <summary>
    /// The enforcement officer says that he or she has reasonable grounds to believe, and does believe, that the above named as the vehicle driver.
    /// </summary>
    [JsonPropertyName("is_driver")]
    public ViolationTicketIsDriver? IsDriver { get; set; }

    /// <summary>
    /// The enforcement officer says that he or she has reasonable grounds to believe, and does believe, that the above named as the vehicle owner.
    /// </summary>
    [JsonPropertyName("is_owner")]
    public ViolationTicketIsOwner? IsOwner { get; set; }

    #endregion

    #region Issuance date, time and location
    /// <summary>
    /// The date and time the violation ticket was issue. Time must only be hours and minutes.
    /// </summary>
    [JsonPropertyName("issued_date")]
    public DateTime? IssuedDate { get; set; }

    /// <summary>
    /// Issued on Road or Highway
    /// </summary>
    [JsonPropertyName("issued_on_road_or_highway")]
    [MaxLength(100)]
    public string? IssuedOnRoadOrHighway { get; set; }

    /// <summary>
    /// Issued at or near city.
    /// </summary>
    [JsonPropertyName("issued_at_or_near_city")]
    [MaxLength(100)]
    public string? IssuedAtOrNearCity { get; set; }
    #endregion

    #region Offsence Details

    /// <summary>
    /// Represents the counts identified. Must have at least one and at most three counts.
    /// </summary>
    [JsonPropertyName("counts")]
    [Required]
    public List<ViolationTicketCount> Counts { get; set; } = new List<ViolationTicketCount>();
    #endregion

    #region
    /// <summary>
    /// The designated provincial court hearing location. For example, Richmond, BC.
    /// </summary>
    [JsonPropertyName("court_location")]
    [MaxLength(100)]
    public string? CourtLocation { get; set; }

    /// <summary>
    /// A unique generated ID set by the system that is used as Redis key for retrieving the ViolationTicket from Redis cache.
    /// </summary>
    [JsonPropertyName("ticket_id")]
    public string? TicketId { get; set; }
    #endregion
}
