namespace TrafficCourts.Messaging.MessageContracts;

/// <summary>
/// A subset of a Disputant's contact information that can be requested to update via a PUT /api/dispute/{guidhash}/contact endpoint.
/// </summary>
public class DisputantUpdateRequest
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

    /// <summary>
    /// represented by lawyer.
    /// </summary>
    public bool? RepresentedByLawyer { get; set; } = null!;

    /// <summary>
    /// Law Firm Name.
    /// </summary>
    public string? LawFirmName { get; set; } = null!;

    /// <summary>
    /// Lawyer surname.
    /// </summary>
    public string? LawyerSurname { get; set; } = null!;

    /// <summary>
    /// Lawyer Given Name 1
    /// </summary>
    public string? LawyerGivenName1 { get; set; } = null!;

    /// <summary>
    /// Lawyer Given Name 2
    /// </summary>
    public string? LawyerGivenName2 { get; set; } = null!;

    /// <summary>
    /// Lawyer Given Name 3
    /// 
    /// </summary>
    public string? LawyerGivenName3 { get; set; } = null!;

    /// <summary>
    /// Lawyer Address
    /// </summary>
    public string? LawyerAddress { get; set; } = null!;

    /// <summary>
    /// Lawyer Phone number
    /// </summary>
    public string? LawyerPhoneNumber { get; set; } = null!;

    /// <summary>
    /// Lawyer Email
    /// </summary>
    public string? LawyerEmail { get; set; } = null!;

    /// <summary>
    /// Interpreter Language Code
    /// </summary>
    public string? InterpreterLanguageCd { get; set; } = null!;

    /// <summary>
    /// Interpreter Required
    /// </summary>
    public bool? InterpreterRequired { get; set; } = null!;

    /// <summary>
    /// number of witnesses
    /// </summary>
    public int? WitnessNo { get; set; } = null!;

    /// <summary>
    /// Fine Reduction Reason
    /// </summary>
    public string? FineReductionReason { get; set; } = null!;

    /// <summary>
    /// Time To Pay Reason
    /// </summary>
    public string? TimeToPayReason { get; set; } = null!;

    /// <summary>
    /// Dispute Counts
    /// </summary>
    public ICollection<Common.OpenAPIs.OracleDataApi.v1_0.DisputeCount>? DisputeCounts { get; set; } = null!;

    /// <summary>
    /// Id of the document uploaded by the disputant
    /// </summary>
    public Guid? DocumentId { get; set; }

    /// <summary>
    /// The type of the document uploaded by the disputant ('Other / Supporting Document' OR 'Application for Adjournment')
    /// </summary>
    public string? DocumentType { get; set; }
}

