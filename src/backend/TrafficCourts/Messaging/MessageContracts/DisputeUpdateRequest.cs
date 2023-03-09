using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;

namespace TrafficCourts.Messaging.MessageContracts;

/// <summary>
/// A subset of a Disputant's contact information that can be requested to update via a PUT /api/dispute/{guidhash}/contact endpoint.
/// </summary>
public class DisputeUpdateRequest : DisputeUpdateContactRequest
{
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
    /// Request Court Appearance
    /// </summary>
    public DisputeRequestCourtAppearanceYn RequestCourtAppearance { get; set; }    
    
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

    /// <summary>
    /// The document uploaded was request to be deleted by the disputant
    /// </summary>
    public Boolean? DocumentDeleteRequested { get; set; }
}