using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;

namespace TrafficCourts.Citizen.Service.Services;

public interface IFormRecognizerService
{

    /// <summary>Analyses the specified image, extracting all text via OCR and mapping results to an OcrViolationTicket.</summary>
    public Task<OcrViolationTicket> AnalyzeImageAsync(MemoryStream stream, CancellationToken cancellationToken);

    // A mapping list of fields extracted from Azure Form Recognizer and their equivalent JSON name from the 2022-04 version of the Violation Ticket image
    protected readonly static Dictionary<string, string> FieldLabels_2022_04 = new ()
    {
        { "Violation Ticket Label", OcrViolationTicket.ViolationTicketTitle },
        { "Violation Ticket Number", OcrViolationTicket.ViolationTicketNumber },
        { "Surname", OcrViolationTicket.Surname },
        { "Given Name", OcrViolationTicket.GivenName },
        { "Drivers Licence Province", OcrViolationTicket.DriverLicenceProvince },
        { "Drivers Licence Number", OcrViolationTicket.DriverLicenceNumber },
        { "Violation Date", OcrViolationTicket.ViolationDate },
        { "Violation Time", OcrViolationTicket.ViolationTime },
        { "Offence is MVA", OcrViolationTicket.OffenceIsMVA },
        { "Offence is MCA", OcrViolationTicket.OffenceIsMCA },
        { "Offence is CTA", OcrViolationTicket.OffenceIsCTA },
        { "Offence is WLA", OcrViolationTicket.OffenceIsWLA },
        { "Offence is FAA", OcrViolationTicket.OffenceIsFAA },
        { "Offence is LCA", OcrViolationTicket.OffenceIsLCA },
        { "Offence is TCR", OcrViolationTicket.OffenceIsTCR },
        { "Offence is Other", OcrViolationTicket.OffenceIsOther },
        { "Count 1 Description", OcrViolationTicket.Count1Description },
        { "Count 1 Act/Regs", OcrViolationTicket.Count1ActRegs },
        { "Count 1 is ACT", OcrViolationTicket.Count1IsACT },
        { "Count 1 is REGS", OcrViolationTicket.Count1IsREGS },
        { "Count 1 Section", OcrViolationTicket.Count1Section },
        { "Count 1 Ticket Amount", OcrViolationTicket.Count1TicketAmount },
        { "Count 2 Description", OcrViolationTicket.Count2Description },
        { "Count 2 Act/Regs", OcrViolationTicket.Count2ActRegs },
        { "Count 2 is ACT", OcrViolationTicket.Count2IsACT },
        { "Count 2 is REGS", OcrViolationTicket.Count2IsREGS },
        { "Count 2 Section", OcrViolationTicket.Count2Section },
        { "Count 2 Ticket Amount", OcrViolationTicket.Count2TicketAmount },
        { "Count 3 Description", OcrViolationTicket.Count3Description },
        { "Count 3 Act/Regs", OcrViolationTicket.Count3ActRegs },
        { "Count 3 is ACT", OcrViolationTicket.Count3IsACT },
        { "Count 3 is REGS", OcrViolationTicket.Count3IsREGS },
        { "Count 3 Section", OcrViolationTicket.Count3Section },
        { "Count 3 Ticket Amount", OcrViolationTicket.Count3TicketAmount },
        { "Hearing Location", OcrViolationTicket.HearingLocation },
        { "Detachment Location", OcrViolationTicket.DetachmentLocation },
        { "Date of Service", OcrViolationTicket.DateOfService }
    };
    
    // A mapping list of fields extracted from Azure Form Recognizer and their equivalent JSON name from the 2023-09 version of the Violation Ticket image
    protected readonly static Dictionary<string, string> FieldLabels_2023_09 = new ()
    {
        { "Label Violation Ticket", OcrViolationTicket.ViolationTicketTitle },
        { "Violation Ticket Number", OcrViolationTicket.ViolationTicketNumber },
        { "Surname", OcrViolationTicket.Surname },
        { "Given Name", OcrViolationTicket.GivenName },
        { "Drivers Licence Province", OcrViolationTicket.DriverLicenceProvince },
        { "Drivers Licence Number", OcrViolationTicket.DriverLicenceNumber },
        { "Violation Date YYYY", OcrViolationTicket.ViolationDateYYYY },
        { "Violation Date MM", OcrViolationTicket.ViolationDateMM },
        { "Violation Date DD", OcrViolationTicket.ViolationDateDD },
        { "Violation Time HH", OcrViolationTicket.ViolationTimeHH },
        { "Violation Time MM", OcrViolationTicket.ViolationTimeMM },
        { "Offence is MVA", OcrViolationTicket.OffenceIsMVA },
        { "Offence is MVAR", OcrViolationTicket.OffenceIsMVAR },
        { "Offence is CCLA", OcrViolationTicket.OffenceIsCCLA },
        { "Offence is CTA", OcrViolationTicket.OffenceIsCTA },
        { "Offence is LCLA", OcrViolationTicket.OffenceIsLCLA },
        { "Offence is TCSR", OcrViolationTicket.OffenceIsTCSR },
        { "Offence is WLA", OcrViolationTicket.OffenceIsWLA },
        { "Offence is FVPA", OcrViolationTicket.OffenceIsFVPA },
        { "Offence is Other", OcrViolationTicket.OffenceIsOther },
        { "Count 1 Description", OcrViolationTicket.Count1Description },
        { "Count 1 is ACT", OcrViolationTicket.Count1IsACT },
        { "Count 1 is REGS", OcrViolationTicket.Count1IsREGS },
        { "Count 1 Section", OcrViolationTicket.Count1Section },
        { "Count 1 Ticket Amount", OcrViolationTicket.Count1TicketAmount },
        { "Count 2 Description", OcrViolationTicket.Count2Description },
        { "Count 2 is ACT", OcrViolationTicket.Count2IsACT },
        { "Count 2 is REGS", OcrViolationTicket.Count2IsREGS },
        { "Count 2 Section", OcrViolationTicket.Count2Section },
        { "Count 2 Ticket Amount", OcrViolationTicket.Count2TicketAmount },
        { "Count 3 Description", OcrViolationTicket.Count3Description },
        { "Count 3 is ACT", OcrViolationTicket.Count3IsACT },
        { "Count 3 is REGS", OcrViolationTicket.Count3IsREGS },
        { "Count 3 Section", OcrViolationTicket.Count3Section },
        { "Count 3 Ticket Amount", OcrViolationTicket.Count3TicketAmount },
        { "Hearing Location", OcrViolationTicket.HearingLocation },
        { "Detachment Location", OcrViolationTicket.DetachmentLocation },
        { "Date of Service", OcrViolationTicket.DateOfService }
    };
}
