namespace TrafficCourts.Messaging.MessageContracts;

/// <summary>
/// A subset of a Disputant's contact information that can be requested to update via a PUT /api/dispute/{guidhash}/contact endpoint.
/// </summary>
public class DisputantUpdateContactRequest
{
    /// <summary>
    /// The notice of dispute identifer.
    /// </summary>
    public Guid NoticeOfDisputeGuid { get; set; }

    /// <summary>
    /// The disputant's email address.
    /// </summary>
    public string? EmailAddress { get; set; } = null!;

    /// <summary>
    /// The first given name or corporate name continued.
    /// </summary>
    public string? DisputantGivenName1 { get; set; } = null!;

    /// <summary>
    /// The second given name
    /// </summary>
    public string? DisputantGivenName2 { get; set; } = null!;

    /// <summary>
    /// The third given name 
    /// </summary>
    public string? DisputantGivenName3 { get; set; } = null!;

    /// <summary>
    /// The surname or corporate name.
    /// </summary>
    public string? DisputantSurname { get; set; } = null!;

    /// <summary>
    /// The mailing address of the disputant.
    /// </summary>
    public string? AddressLine1 { get; set; } = null!;

    /// <summary>
    /// The mailing address of the disputant.
    /// </summary>
    public string? AddressLine2 { get; set; } = null!;

    /// <summary>
    /// The mailing address of the disputant.
    /// </summary>
    public string? AddressLine3 { get; set; } = null!;

    /// <summary>
    /// The mailing address city of the disputant.
    /// </summary>
    public string? AddressCity { get; set; } = null!;

    /// <summary>
    /// The mailing address province of the disputant.
    /// </summary>
    public string? AddressProvince { get; set; } = null!;

    /// <summary>
    /// The mailing address postal code or zip code of the disputant.
    /// </summary>
    public string? PostalCode { get; set; } = null!;

    /// <summary>
    /// The mailing address province's country code of the disputant.
    /// </summary>
    public int? AddressProvinceCountryId { get; set; }

    /// <summary>
    /// The mailing address province's sequence number of the disputant.
    /// </summary>
    public int? AddressProvinceSeqNo { get; set; }

    /// <summary>
    /// The mailing address country id of the disputant.
    /// </summary>
    public int? AddressCountryId { get; set; }

    /// <summary>
    /// The disputant's home phone number.
    /// </summary>
    public string? HomePhoneNumber { get; set; } = null!;
}

