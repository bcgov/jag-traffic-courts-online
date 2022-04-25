using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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

    #region Fields related to the Issued To section
    /// <summary>
    /// The surname or corporate name.
    /// </summary>
    [JsonPropertyName("surname")]
    [MaxLength(30)]
    public string? Surname { get; set; }

    /// <summary>
    /// The given names or corporate name continued.
    /// </summary>
    [JsonPropertyName("given_names")]
    [MaxLength(30)]
    public string? GivenNames { get; set; }

    /// <summary>
    /// The person issued the ticket has been identified as a young person.
    /// </summary>
    [JsonPropertyName("is_young_person")]
    public bool? IsYoungPerson { get; set; }

    #region Drivers Licence

    /// <summary>
    /// The drivers licence number. Note not all jurisdictions will use numeric drivers licence numbers.
    /// </summary>
    [JsonPropertyName("drivers_licence_number")]
    [MaxLength(20)]
    public string? DriversLicenceNumber { get; set; }

    /// <summary>
    /// The province or state the drivers licence was issued by.
    /// </summary>
    [JsonPropertyName("drivers_licence_province")]
    [MaxLength(30)]
    public string? DriversLicenceProvince { get; set; }

    /// <summary>
    /// The year the drivers licence was produced.
    /// </summary>
    [JsonPropertyName("drivers_licence_produced_year")]
    [Range(1900, 2100)]
    public short? DriversLicenceProducedYear { get; set; }

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
    [JsonPropertyName("birthdate")]
    [SwaggerSchema(Format = "date")]
    public DateTime? Birthdate { get; set; }

    /// <summary>
    /// The address of the individual the violation ticket was issued to.
    /// </summary>
    [JsonPropertyName("address")]
    [MaxLength(30)]
    public string? Address { get; set; }

    /// <summary>
    /// The city of the individual the violation ticket was issued to.
    /// </summary>
    [JsonPropertyName("city")]
    [MaxLength(30)]
    public string? City { get; set; }

    /// <summary>
    /// The province or state of the individual the violation ticket was issued to.
    /// </summary>
    [JsonPropertyName("province")]
    [MaxLength(30)]
    public string? Province { get; set; }

    /// <summary>
    /// The postal code or zip code.
    /// </summary>
    [JsonPropertyName("postal_code")]
    [MaxLength(6)]
    public string? PostalCode { get; set; }

    /// <summary>
    /// The address represents a change of address. The address on the violation would be different from the address 
    /// on the drivers licence or provided identification.
    /// </summary>
    [JsonPropertyName("is_change_of_address")]
    public bool? IsChangeOfAddress { get; set; }

    #endregion

    #region Issued To designation
    /// <summary>
    /// The enforcement officer says that he or she has reasonable grounds to believe, and does believe, that the above named as the vehicle driver.
    /// </summary>
    [JsonPropertyName("is_driver")]
    public bool? IssuedToIsDriver { get; set; }
    /// <summary>
    /// The enforcement officer says that he or she has reasonable grounds to believe, and does believe, that the above named as the cyclist.
    /// </summary>
    [JsonPropertyName("is_cyclist")]
    public bool? IssuedToCyclist { get; set; }
    /// <summary>
    /// The enforcement officer says that he or she has reasonable grounds to believe, and does believe, that the above named as the vehicle owner.
    /// </summary>
    [JsonPropertyName("is_owner")]
    public bool? IssuedToOwner { get; set; }
    /// <summary>
    /// The enforcement officer says that he or she has reasonable grounds to believe, and does believe, that the above named as a pedestrain.
    /// </summary>
    [JsonPropertyName("is_pedestrain")]
    public bool? IssuedToPedestrain { get; set; }
    /// <summary>
    /// The enforcement officer says that he or she has reasonable grounds to believe, and does believe, that the above named as a passenger.
    /// </summary>
    [JsonPropertyName("is_passenger")]
    public bool? IssuedToPassenger { get; set; }
    /// <summary>
    /// The enforcement officer says that he or she has reasonable grounds to believe, and does believe, that the above named as a other designation.
    /// </summary>
    [JsonPropertyName("is_other")]
    public bool? IssuedToOther { get; set; }

    /// <summary>
    /// If <see cref="IssuedToOther"/> is true, the other designation description.
    /// </summary>
    [JsonPropertyName("other_description")]
    [MaxLength(100)]
    public string? IssuedToOtherDescription { get; set; }
    #endregion

    #region Issuance date, time and location
    /// <summary>
    /// The date and time the violation ticket was issue. Time must only be hours and minutes.
    /// </summary>
    [JsonPropertyName("issued_date")]
    public DateTime? IssuedDate { get; set; }

    /// <summary>
    /// The violation ticket was issued on this road or highway.
    /// </summary>
    [JsonPropertyName("issued_on_road_or_highway")]
    [MaxLength(100)]
    public string? IssuedOnRoadOrHighway { get; set; }

    /// <summary>
    /// The violation ticket was issued at or near this city or town.
    /// </summary>
    [JsonPropertyName("issued_at_or_near_city")]
    [MaxLength(100)]
    public string? IssuedAtOrNearCityOrTown { get; set; }
    #endregion

    #region Offsence Details

    /// <summary>
    /// The violation ticket was issued for offence under the Motor Vehicle Act (MVA).
    /// </summary>
    [JsonPropertyName("is_mva_offence")]
    public bool? IsMvaOffence { get; set; }

    /// <summary>
    /// The violation ticket was issued for offence under the Wildlife Act (WLA).
    /// </summary>
    [JsonPropertyName("is_wla_offence")]
    public bool? IsWlaOffence { get; set; }

    /// <summary>
    /// The violation ticket was issued for offence under the Liquor Control and Licencing Act (LCA).
    /// </summary>
    [JsonPropertyName("is_lca_offence")]
    public bool? IsLcaOffence { get; set; }

    /// <summary>
    /// The violation ticket was issued for offence under the Motor Carrier Act (MCA).
    /// </summary>
    [JsonPropertyName("is_mca_offence")]
    public bool? IsMcaOffence { get; set; }

    /// <summary>
    /// The violation ticket was issued for offence under the Firearm Act (FAA).
    /// </summary>
    [JsonPropertyName("is_faa_offence")]
    public bool? IsFaaOffence { get; set; }

    /// <summary>
    /// The violation ticket was issued for offence under the Transit Conduct and Safety Regs (TCR).
    /// </summary>
    [JsonPropertyName("is_tcr_offence")]
    public bool? IsTcrOffence { get; set; }

    /// <summary>
    /// The violation ticket was issued for offence under the Commercial Transport Act (CTA).
    /// </summary>
    [JsonPropertyName("is_cta_offence")]
    public bool? IsCtaOffence { get; set; }

    /// <summary>
    /// The violation ticket was issued for other.
    /// </summary>
    [JsonPropertyName("is_other_offence")]
    public bool? IsOtherOffence { get; set; }

    /// <summary>
    /// The violation ticket was issued for other.
    /// </summary>
    [JsonPropertyName("other_offence_description")]
    [MaxLength(100)]
    public string? OtherOffenceDescription { get; set; }

    /// <summary>
    /// Represents the counts identified. Must have at least one and at most three counts.
    /// </summary>
    [JsonPropertyName("counts")]
    [Required]
    [Range(1, 3)]
    public List<ViolationTicketCount> Counts { get; set; } = new List<ViolationTicketCount>();
    #endregion

    #region
    /// <summary>
    /// The designated provincial court hearing location. For example, Richmond, BC.
    /// </summary>
    [JsonPropertyName("provincial_court_hearing_location")]
    [MaxLength(100)]
    public string? ProvincialCourtHearingLocation { get; set; }

    /// <summary>
    /// The organization or detatchment location. For example, Delta Police.
    /// </summary>
    [JsonPropertyName("organization_location")]
    [MaxLength(100)]
    public string? OrganizationLocation { get; set; }
    #endregion
}
