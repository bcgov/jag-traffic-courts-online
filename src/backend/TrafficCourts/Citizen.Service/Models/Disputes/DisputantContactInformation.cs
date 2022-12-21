using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TrafficCourts.Citizen.Service.Models.Disputes;

/// <summary>
/// A subset of a Disputant's contact information that can be requested to update via a PUT /api/dispute/{guidhash}/contact endpoint.
/// </summary>
public class DisputantContactInformation
{
    /// <summary>
    /// The disputant's email address.
    /// </summary>
    [JsonPropertyName("email_address")]
    [MaxLength(100)]
    public string? EmailAddress { get; set; } = null!;

    /// <summary>
    /// The first given name or corporate name continued.
    /// </summary>
    [JsonPropertyName("disputant_given_name1")]
    [MaxLength(30)]
    public string? DisputantGivenName1 { get; set; } = null!;

    /// <summary>
    /// The second given name
    /// </summary>
    [JsonPropertyName("disputant_given_name2")]
    [MaxLength(30)]
    public string? DisputantGivenName2 { get; set; } = null!;

    /// <summary>
    /// The third given name 
    /// </summary>
    [JsonPropertyName("disputant_given_name3")]
    [MaxLength(30)]
    public string? DisputantGivenName3 { get; set; } = null!;

    /// <summary>
    /// The surname or corporate name.
    /// </summary>
    [JsonPropertyName("disputant_surname")]
    [MaxLength(30)]
    public string? DisputantSurname { get; set; } = null!;

    /// <summary>
    /// The mailing address of the disputant.
    /// </summary>
    [JsonPropertyName("address_line1")]
    [MaxLength(100)]
    public string? AddressLine1 { get; set; } = null!;

    /// <summary>
    /// The mailing address of the disputant.
    /// </summary>
    [JsonPropertyName("address_line2")]
    [MaxLength(100)]
    public string? AddressLine2 { get; set; } = null!;

    /// <summary>
    /// The mailing address of the disputant.
    /// </summary>
    [JsonPropertyName("address_line3")]
    [MaxLength(100)]
    public string? AddressLine3 { get; set; } = null!;

    /// <summary>
    /// The mailing address city of the disputant.
    /// </summary>
    [JsonPropertyName("address_city")]
    [MaxLength(30)]
    public string? AddressCity { get; set; } = null!;

    /// <summary>
    /// The mailing address province of the disputant.
    /// </summary>
    [JsonPropertyName("address_province")]
    [MaxLength(30)]
    public string? AddressProvince { get; set; } = null!;

    /// <summary>
    /// The mailing address province's country code of the disputant.
    /// </summary>
    [JsonPropertyName("address_province_country_id")]
    public int? AddressProvinceCountryId { get; set; }

    /// <summary>
    /// The mailing address province's sequence number of the disputant.
    /// </summary>
    [JsonPropertyName("address_province_seq_no")]
    public int? AddressProvinceSeqNo { get; set; }

    /// <summary>
    /// The mailing address country id of the disputant.
    /// </summary>
    [JsonPropertyName("address_country_id")]
    public int? AddressCountryId { get; set; }

    /// <summary>
    /// The mailing address postal code or zip code of the disputant.
    /// </summary>
    [JsonPropertyName("postal_code")]
    [MaxLength(10)]
    public string? PostalCode { get; set; } = null!;

    /// <summary>
    /// The disputant's home phone number.
    /// </summary>
    [JsonPropertyName("home_phone_number")]
    [MaxLength(20)]
    public string? HomePhoneNumber { get; set; } = null!;
}

