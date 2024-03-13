using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace TrafficCourts.Domain.Models;

/// <summary>
/// A model representation of the extracted OCR results.
/// </summary>
[ExcludeFromCodeCoverage]
public class OcrViolationTicket
{

    /// <summary>
    /// A Violation Ticket form that was in circulation around 2022.04
    /// </summary>
    public static readonly string ViolationTicketVersion1_x = "ViolationTicket:ViolationTicket_v1";
    public static readonly string ViolationTicketVersion1_beta = "Violation Ticket:Violation Ticket 2022.04";

    /// <summary>
    /// A new Violation Ticket form that was in introduced in 2023.09
    /// </summary>
    public static readonly string ViolationTicketVersion2_x = "ViolationTicket:ViolationTicket_v2";
    public static readonly string ViolationTicketVersion2_beta = "Violation Ticket:Violation Ticket 2023.09";

    public static readonly string ViolationTicketTitle = "violationTicketTitle";
    public static readonly string ViolationTicketNumber = "ticket_number";
    public static readonly string Version = "ticket_version";
    public static readonly string Surname = "disputant_surname";
    public static readonly string GivenName = "disputant_given_names";
    public static readonly string DriverLicenceProvince = "drivers_licence_province";
    public static readonly string DriverLicenceNumber = "drivers_licence_number";
    public static readonly string ViolationDate = "violation_date";
    public static readonly string ViolationDateYYYY = "violation_date_yyyy";
    public static readonly string ViolationDateMM = "violation_date_mm";
    public static readonly string ViolationDateDD = "violation_date_dd";
    public static readonly string ViolationTime = "violation_time";
    public static readonly string ViolationTimeHH = "violation_time_hh";
    public static readonly string ViolationTimeMM = "violation_time_mm";
    public static readonly string OffenceIsMVA = "is_mva_offence";
    public static readonly string OffenceIsMCA = "is_mca_offence";
    public static readonly string OffenceIsCTA = "is_cta_offence";
    public static readonly string OffenceIsWLA = "is_wla_offence";
    public static readonly string OffenceIsFAA = "is_faa_offence";
    public static readonly string OffenceIsLCA = "is_lca_offence";
    public static readonly string OffenceIsTCR = "is_tcr_offence";
    public static readonly string OffenceIsMVAR = "is_mvar_offence";
    public static readonly string OffenceIsCCLA = "is_ccla_offence";
    public static readonly string OffenceIsLCLA = "is_lcla_offence";
    public static readonly string OffenceIsTCSR = "is_tcsr_offence";
    public static readonly string OffenceIsFVPA = "is_fvpa_offence";
    public static readonly string OffenceIsOther = "is_other_offence";
    public static readonly string Count1Description = "counts.count_no_1.description";
    public static readonly string Count1ActRegs = "counts.count_no_1.act_or_regulation_name_code";
    public static readonly string Count1IsACT = "counts.count_no_1.is_act";
    public static readonly string Count1IsREGS = "counts.count_no_1.is_regulation";
    public static readonly string Count1Section = "counts.count_no_1.section";
    public static readonly string Count1TicketAmount = "counts.count_no_1.ticketed_amount";
    public static readonly string Count2Description = "counts.count_no_2.description";
    public static readonly string Count2ActRegs = "counts.count_no_2.act_or_regulation_name_code";
    public static readonly string Count2IsACT = "counts.count_no_2.is_act";
    public static readonly string Count2IsREGS = "counts.count_no_2.is_regulation";
    public static readonly string Count2Section = "counts.count_no_2.section";
    public static readonly string Count2TicketAmount = "counts.count_no_2.ticketed_amount";
    public static readonly string Count3Description = "counts.count_no_3.description";
    public static readonly string Count3ActRegs = "counts.count_no_3.act_or_regulation_name_code";
    public static readonly string Count3IsACT = "counts.count_no_3.is_act";
    public static readonly string Count3IsREGS = "counts.count_no_3.is_regulation";
    public static readonly string Count3Section = "counts.count_no_3.section";
    public static readonly string Count3TicketAmount = "counts.count_no_3.ticketed_amount";
    public static readonly string HearingLocation = "court_location";
    public static readonly string DetachmentLocation = "detachment_location";
    public static readonly string DateOfService = "service_date";

    /// <summary>
    /// Gets of sets the version of this ViolationTicket
    /// </summary>
    public ViolationTicketVersion TicketVersion { get; set; }

    /// <summary>
    /// Gets or sets the saved image filename.
    /// </summary>
    public string? ImageFilename { get; set; }

    /// <summary>
    /// A global confidence of correctly extracting the document. This value will be low if the title of this 
    /// Violation Ticket form is not found (or of low confidence itself) or if the main ticket number is missing or invalid.
    /// </summary>
    public float GlobalConfidence { get; set; }

    /// <summary>
    /// A list of global reasons why the global Confidence may be low (ie, missing ticket number, not a Violation Ticket, etc.)
    /// </summary>
    [JsonIgnore]
    public List<string> GlobalValidationErrors { get; set; } = new List<string>();

    /// <summary>
    /// An enumeration of all fields in this Violation Ticket.
    /// </summary>
    public Dictionary<string, Field> Fields { get; set; } = new Dictionary<string, Field>();

    /// <summary>
    /// Return true if any of the 4 Count fields has a value.
    /// </summary>
    public bool IsCount1Populated()
    {
        return Fields[Count1Description].IsPopulated()
            || (ViolationTicketVersion.VT1 == TicketVersion && Fields[Count1ActRegs].IsPopulated())
            || Fields[Count1Section].IsPopulated()
            || Fields[Count1TicketAmount].IsPopulated();
    }

    /// <summary>
    /// Return true if any of the 4 Count fields has a value.
    /// </summary>
    public bool IsCount2Populated()
    {
        return Fields[Count2Description].IsPopulated()
            || (ViolationTicketVersion.VT1 == TicketVersion && Fields[Count2ActRegs].IsPopulated())
            || Fields[Count2Section].IsPopulated()
            || Fields[Count2TicketAmount].IsPopulated();
    }

    /// <summary>
    /// Return true if any of the 4 Count fields has a value.
    /// </summary>
    public bool IsCount3Populated()
    {
        return Fields[Count3Description].IsPopulated()
            || (ViolationTicketVersion.VT1 == TicketVersion && Fields[Count3ActRegs].IsPopulated())
            || Fields[Count3Section].IsPopulated()
            || Fields[Count3TicketAmount].IsPopulated();
    }
}
