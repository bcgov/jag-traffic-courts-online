using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TrafficCourts.Cdogs.Client;
using TrafficCourts.Common.Features.Lookups;
using TrafficCourts.Common.Models;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Staff.Service.Models.DigitalCaseFiles.Print;

namespace TrafficCourts.Staff.Service.Services;

public class PrintDigitalCaseFileService : IPrintDigitalCaseFileService
{
    private readonly IJJDisputeService _disputeService;
    private readonly IOracleDataApiClient _oracleDataApi;
    private readonly IProvinceLookupService _provinceLookupService;
    private readonly IDocumentGenerationService _documentGeneration;
    private readonly ILogger<PrintDigitalCaseFileService> _logger;

    public PrintDigitalCaseFileService(
        IJJDisputeService disputeService,
        IOracleDataApiClient oracleDataApi,
        IProvinceLookupService provinceLookupService,
        IDocumentGenerationService documentGeneration,
        ILogger<PrintDigitalCaseFileService> logger)
    {
        _disputeService = disputeService ?? throw new ArgumentNullException(nameof(disputeService));
        _oracleDataApi = oracleDataApi ?? throw new ArgumentNullException(nameof(oracleDataApi));
        _provinceLookupService = provinceLookupService ?? throw new ArgumentNullException(nameof(provinceLookupService));
        _documentGeneration = documentGeneration ?? throw new ArgumentNullException(nameof(documentGeneration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Renders the digital case file for a given dispute based on ticket number. This really should be using the tco_dispute.dispute_id.
    /// </summary>
    public async Task<RenderedReport> PrintDigitalCaseFileAsync(string ticketNumber, string timeZoneId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(ticketNumber);
        ArgumentNullException.ThrowIfNull(timeZoneId);

        // generate the digital case file model
        DigitalCaseFile digitalCaseFile = await GetDigitalCaseFileAsync(ticketNumber, timeZoneId, cancellationToken);

        var report = await RenderReportAsync(digitalCaseFile, cancellationToken);

        return report;
    }


    private async Task<RenderedReport> RenderReportAsync(DigitalCaseFile digitalCaseFile, CancellationToken cancellationToken)
    {
        // get the template
        Stream template = GetTemplate("template_DigitalCaseFile.docx");
        var templateType = TemplateType.Word;

        var convertTo = ConvertTo.Pdf;
        var reportName = $"DCF {digitalCaseFile.Ticket.Number}.pdf";

        // render the report
        var report = await _documentGeneration.UploadTemplateAndRenderReportAsync(template, templateType, convertTo, reportName, digitalCaseFile, cancellationToken);

        return report;
    }

    private Stream GetTemplate(string name)
    {
        string path = $"Models.DigitalCaseFiles.Print.Templates.{name}";
        EmbeddedFileProvider fileProvider = new EmbeddedFileProvider(typeof(DigitalCaseFile).Assembly);
        IFileInfo fileInfo = fileProvider.GetFileInfo(path);

        if (!fileInfo.Exists)
        {
            // throw TemplateNotFoundExeception
        }

        Stream stream = fileInfo.CreateReadStream();
        return stream;
    }

    private async Task<Province?> GetDriversLicenceProvinceAsync(string provinceSeqNo, string countryId)
    {
        Province? driversLicenceProvince = null;
        if (provinceSeqNo is not null && countryId is not null)
        {
            driversLicenceProvince = await _provinceLookupService.GetByProvSeqNoCtryIdAsync(provinceSeqNo, countryId);
        }

        return driversLicenceProvince;
    }

    /// <summary>
    /// Fetches the <see cref="DigitalCaseFile"/> based on ticket number. This really should be using the tco_dispute.dispute_id.
    /// </summary>
    internal async Task<DigitalCaseFile> GetDigitalCaseFileAsync(string ticketNumber, string timeZoneId, CancellationToken cancellationToken)
    {
        // JavaScript: Intl.DateTimeFormat().resolvedOptions().timeZone
        // Time Zone from the browser is either a time zone identifier from the IANA Time Zone Database or a UTC offset in ISO 8601 extended format.
        // https://tc39.es/ecma402/#sec-properties-of-intl-datetimeformat-instances

        // get the user's time zone
        TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

        var dispute = await _disputeService.GetJJDisputeAsync(ticketNumber, false, cancellationToken);
        
        Province? driversLicenceProvince = await GetDriversLicenceProvinceAsync(dispute.DrvLicIssuedProvSeqNo, dispute.DrvLicIssuedCtryId);
        var fileHistory = await _oracleDataApi.GetFileHistoryByTicketNumberAsync(dispute.TicketNumber, cancellationToken);

        var digitalCaseFile = new DigitalCaseFile();

        // fill in each section, the sections and fields are populated in order matching the template

        // set the ticket information
        var ticket = digitalCaseFile.Ticket;
        ticket.Number = dispute.TicketNumber;
        ticket.Surname = dispute.OccamDisputantSurnameNm;
        ticket.GivenNames = ConcatenateWithSpaces(dispute.OccamDisputantGiven1Nm, dispute.OccamDisputantGiven2Nm, dispute.OccamDisputantGiven3Nm);
        ticket.OffenceLocation = dispute.OffenceLocation;
        ticket.PoliceDetachment = dispute.PoliceDetachment;
        ticket.Issued = new FormattedDateTime(dispute.IssuedTs); 
        ticket.Submitted = new FormattedDateOnly(dispute.SubmittedTs);
        ticket.IcbcReceived = new FormattedDateOnly(dispute.IcbcReceivedDate);
        ticket.CourtAgenyId = dispute.CourtAgenId;
        ticket.CourtHouse = dispute.CourthouseLocation;

        // set the contact information
        var contact = digitalCaseFile.Contact;
        contact.Surname = dispute.ContactSurname ?? ticket.Surname;
        contact.GivenNames = ConcatenateWithSpaces(dispute.ContactGivenName1, dispute.ContactGivenName2, dispute.ContactGivenName3);
        if (string.IsNullOrEmpty(contact.GivenNames))
        {
            contact.GivenNames = ticket.GivenNames;
        }
        contact.Address = FormatAddress(dispute);
        contact.DriversLicence.Province = driversLicenceProvince?.ProvAbbreviationCd ?? string.Empty;
        contact.DriversLicence.Number = dispute.DriversLicenceNumber;
        contact.Email = dispute.EmailAddress;

        // set the court options
        var options = digitalCaseFile.CourtOptions;
        options.LegalCounsel.FullName = ConcatenateWithSpaces(dispute.LawyerGivenName1, dispute.LawyerGivenName2, dispute.LawyerGivenName3, dispute.LawyerSurname);
        options.LegalCounsel.FirmName = dispute.LawFirmName;
        options.WitnessCount = dispute.WitnessNo ?? 0;
        options.InterpreterLanguage = dispute.InterpreterLanguageCd; // code => description?
        options.DisputantAttendanceType = ToString(dispute.DisputantAttendanceType);

        // set the court appearance
        Appearance appearance = digitalCaseFile.Appearance;
        // Get the JJDisputeCourtAppearanceRoP with the most recent AppearanceTs.
        JJDisputeCourtAppearanceRoP currentAppearance = dispute.JjDisputeCourtAppearanceRoPs.OrderByDescending(a => a.AppearanceTs).First();
        // get display name of JJ based on their IDIR
        string? jjDisplayName = string.Empty;
        if (dispute.JjAssignedTo is not null)
        {
            jjDisplayName = await _disputeService.GetDisputeAssignToDisplayNameAsync(dispute.JjAssignedTo, cancellationToken);
        }  

        SetFields(appearance, currentAppearance, jjDisplayName);

        // set the court appearance history
        var appearanceHistory = digitalCaseFile.AppearanceHistory;
        foreach (var rop in dispute.JjDisputeCourtAppearanceRoPs.Where(_ => _ != currentAppearance).OrderByDescending(a => a.AppearanceTs))
        {
            // TODO: how do we know the current appearance vs historical ones?
            appearanceHistory.Add(SetFields(new Appearance(), rop, null));
        }

        // set written reasons
        var writtenReasons = digitalCaseFile.WrittenReasons;
        writtenReasons.FineReduction = dispute.FineReductionReason;
        writtenReasons.TimeToPay = dispute.TimeToPayReason;

        // set the counts
        var counts = digitalCaseFile.Counts;
        for (int i = 1; i <= 3; i++)
        {
            var offenseCount = new OffenseCount();

            var disputedCount = dispute.JjDisputedCounts.FirstOrDefault(_ => _.Count == i);
            if (disputedCount is not null)
            {
                offenseCount.Count = disputedCount.Count.ToString();
                offenseCount.Offense = new FormattedDateOnly(dispute.IssuedTs);
                offenseCount.Plea = ToString(disputedCount.Plea);
                offenseCount.Description = disputedCount.Description;
                offenseCount.Due = new FormattedDateOnly(disputedCount.DueDate);
                offenseCount.Fine = (decimal?)(disputedCount.TicketedFineAmount);
                offenseCount.AppearInCourt = ToString(disputedCount.AppearInCourt);
                offenseCount.RequestFineReduction = ToString(disputedCount.RequestReduction);
                offenseCount.RequestTimeToPay = ToString(disputedCount.RequestTimeToPay);
                offenseCount.ReviseFine = SetReviseFine(disputedCount);
                offenseCount.LesserOrGreaterAmount = (decimal?)(disputedCount.LesserOrGreaterAmount);
                offenseCount.IncludesSurcharge = ToString(disputedCount.IncludesSurcharge);
                offenseCount.RoundLesserOrGreaterAmount = disputedCount.LesserOrGreaterAmount != null
                    ? Math.Round((decimal)disputedCount.LesserOrGreaterAmount / (offenseCount.IncludesSurcharge == "Y" ? 1.15M : 1M))
                    : null;
                offenseCount.TotalFineAmount = (decimal?)(disputedCount.TotalFineAmount ?? disputedCount.TicketedFineAmount);
                offenseCount.IsDueDateRevised = IsDueDateRevised(disputedCount);
                offenseCount.RevisedDue = IsDueDateRevised(disputedCount) ? new FormattedDateOnly(disputedCount.RevisedDueDate) : new FormattedDateOnly(disputedCount.DueDate);
                offenseCount.FinalDue = disputedCount.RevisedDueDate != null ? new FormattedDateOnly(disputedCount.RevisedDueDate) : new FormattedDateOnly(disputedCount.DueDate);
                offenseCount.Surcharge = offenseCount.RoundLesserOrGreaterAmount != null 
                    ? Math.Round((decimal)offenseCount.RoundLesserOrGreaterAmount * 0.15M) : 0;
                offenseCount.Comments = disputedCount.Comments;
                // set jjDisputedCountRoP data for this count
                offenseCount.Finding = ToString(disputedCount.JjDisputedCountRoP.Finding);
                offenseCount.LesserDescription = disputedCount.JjDisputedCountRoP.LesserDescription;
                offenseCount.SsProbationDuration = disputedCount.JjDisputedCountRoP.SsProbationDuration;
                offenseCount.SsProbationConditions = disputedCount.JjDisputedCountRoP.SsProbationConditions;
                offenseCount.JailDuration = disputedCount.JjDisputedCountRoP.JailDuration;
                offenseCount.JailIntermittent = ToString(disputedCount.JjDisputedCountRoP.JailIntermittent);
                offenseCount.ProbationDuration = disputedCount.JjDisputedCountRoP.ProbationDuration;
                offenseCount.ProbationConditions = disputedCount.JjDisputedCountRoP.ProbationConditions;
                offenseCount.DrivingProhibitionDuration = disputedCount.JjDisputedCountRoP.DrivingProhibition;
                offenseCount.DrivingProhibitionMVA = disputedCount.JjDisputedCountRoP.DrivingProhibitionMVASection;
                offenseCount.Dismissed = ToString(disputedCount.JjDisputedCountRoP.Dismissed);
                offenseCount.WantOfProsecution = ToString(disputedCount.JjDisputedCountRoP.ForWantOfProsecution);
                offenseCount.Withdrawn = ToString(disputedCount.JjDisputedCountRoP.Withdrawn);
                offenseCount.Abatement = ToString(disputedCount.JjDisputedCountRoP.Abatement);
                offenseCount.StayOfProceedingsBy = disputedCount.JjDisputedCountRoP.StayOfProceedingsBy;
                offenseCount.Other = disputedCount.JjDisputedCountRoP.Other;
            }

            counts.Add(offenseCount);
        }

        // set justin documents
        digitalCaseFile.IsElectronicTicket = ToString(dispute.ElectronicTicketYn);
        digitalCaseFile.HasNoticeOfHearing = ToString(dispute.NoticeOfHearingYn);

        // set uploaded documents
        var documents = digitalCaseFile.Documents;
        if (dispute.FileData != null)
        {
            foreach (var fileData in dispute.FileData.Where(_ => _.FileName != null))
            {
#pragma warning disable CS8601 // Possible null reference assignment - already checked FileName != null
                documents.Add(new Document { FileName = fileData.FileName });
#pragma warning restore CS8601 // Possible null reference assignment.
            }
        }

        // set file history
        var history = digitalCaseFile.History;

        // File History searches by ticket number and *could* return files for multiple disputes
        // This is a bug waiting to happen, so we will be more careful
        foreach (var h in fileHistory.Where(_ => _.DisputeId == dispute.OccamDisputeId))
        {
            history.Add(new FileHistoryEvent
            {
                When = new FormattedDateTime(h.CreatedTs),
                Description = h.Description,
                Type = h.AuditLogEntryType.ToString(),
                Username = h.ActionByApplicationUser,
            });
        }

        // set file remarks
        var remarks = digitalCaseFile.FileRemarks;
        foreach (var remark in dispute.Remarks)
        {
            remarks.Add(new FileRemark
            {
                When = new FormattedDateTime(remark.RemarksMadeTs),
                Username = remark.UserFullName,
                Note = remark.Note,
            });
        }

        return digitalCaseFile;
    }

    private Appearance SetFields(Appearance appearance, JJDisputeCourtAppearanceRoP appearanceRop, string? jjDisplayName)
    {
        appearance.When = new FormattedDateTime(appearanceRop.AppearanceTs);
        appearance.Room = appearanceRop.Room;
        appearance.Reason = appearanceRop.Reason;
        appearance.App = ToString(appearanceRop.AppCd);
        appearance.NoApp = new FormattedDateTime(appearanceRop.NoAppTs);
        appearance.Clerk = appearanceRop.ClerkRecord;
        appearance.DefenseCouncil = appearanceRop.DefenceCounsel;
        appearance.DefenseAtt = ToString(appearanceRop.DattCd);
        appearance.Crown = ToString(appearanceRop.Crown);
        appearance.Seized = ToString(appearanceRop.JjSeized);
        appearance.JudicialJustice = jjDisplayName ?? appearanceRop.Adjudicator;
        appearance.Comments = appearanceRop.Comments;

        return appearance;
    }

    private string ToString(JJDisputedCountRequestReduction? reduction, JJDisputedCountRequestTimeToPay timeToPay)
    {
        var requestReduction = ToString(reduction);
        var requestRimeToPay = ToString(timeToPay);

        if (requestReduction == string.Empty && requestRimeToPay == string.Empty)
        {
            return string.Empty;
        }

        if (requestReduction == "Yes" && requestRimeToPay == "Yes")
        {
            return "Fine reduction and time to pay";
        }
        else if (requestReduction == "Yes" && requestRimeToPay == "No")
        {
            return "Fine reduction";
        }
        else if (requestReduction == "No" && requestRimeToPay == "Yes")
        {
            return "Time to pay";
        }

        return string.Empty; // shouldn't happen
    }

    private string ToString(JJDisputedCountPlea? value)
    {
        if (value is not null && value != JJDisputedCountPlea.UNKNOWN)
        {
            switch (value) 
            {
                case JJDisputedCountPlea.G: return "Guilty";
                case JJDisputedCountPlea.N: return "Not Guilty";
            }
        }

        return string.Empty;
    }

    private string ToString(JJDisputedCountRequestTimeToPay? value)
    {
        if (value is not null && value != JJDisputedCountRequestTimeToPay.UNKNOWN)
        {
            switch (value)
            {
                case JJDisputedCountRequestTimeToPay.Y: return "Yes";
                case JJDisputedCountRequestTimeToPay.N: return "No";
            }
        }

        return string.Empty;
    }

    private string ToString(JJDisputedCountRequestReduction? value)
    {
        if (value is not null && value != JJDisputedCountRequestReduction.UNKNOWN)
        {
            switch (value)
            {
                case JJDisputedCountRequestReduction.Y: return "Yes";
                case JJDisputedCountRequestReduction.N: return "No";
            }
        }

        return string.Empty;
    }

    private string ToString(JJDisputeNoticeOfHearingYn? value)
    {
        if (value is not null && value != JJDisputeNoticeOfHearingYn.UNKNOWN)
        {
            return value.Value.ToString();
        }

        return string.Empty;
    }

    private string ToString(JJDisputeElectronicTicketYn? value)
    {
        if (value is not null && value != JJDisputeElectronicTicketYn.UNKNOWN)
        {
            return value.Value.ToString();
        }

        return string.Empty;
    }

    private string ToString(JJDisputeCourtAppearanceRoPJjSeized? value)
    {
        if (value is not null && value != JJDisputeCourtAppearanceRoPJjSeized.UNKNOWN)
        {
            return value.Value.ToString();
        }

        return string.Empty;
    }

    private string ToString(JJDisputeCourtAppearanceRoPCrown? value)
    {
        if (value is not null && value != JJDisputeCourtAppearanceRoPCrown.UNKNOWN)
        {
            return value.Value.ToString();
        }

        return string.Empty;
    }

    private string ToString(JJDisputeCourtAppearanceRoPDattCd? value)
    {
        if (value is not null && value != JJDisputeCourtAppearanceRoPDattCd.UNKNOWN)
        {
            return value.Value.ToString();
        }

        return string.Empty;
    }

    private string ToString(JJDisputeCourtAppearanceRoPAppCd? value)
    {
        if (value is not null && value != JJDisputeCourtAppearanceRoPAppCd.UNKNOWN)
        {
            return value.Value.ToString(); 
        }

        return string.Empty;

    }
    private string ToString(JJDisputeDisputantAttendanceType? value)
    {
        if (value is not null && value != JJDisputeDisputantAttendanceType.UNKNOWN)
        {
            switch (value)
            {
                case JJDisputeDisputantAttendanceType.WRITTEN_REASONS: return "Written Reasons";
                case JJDisputeDisputantAttendanceType.IN_PERSON: return "In Person";
                case JJDisputeDisputantAttendanceType.MSTEAMS_AUDIO: return "Teams - Audio";
                case JJDisputeDisputantAttendanceType.MSTEAMS_VIDEO: return "Teams - Video";
                case JJDisputeDisputantAttendanceType.TELEPHONE_CONFERENCE: return "Telephone Conference";
                case JJDisputeDisputantAttendanceType.VIDEO_CONFERENCE: return "Video Conference";
            }
        }

        return string.Empty;
    }

    private string ToString(JJDisputedCountRoPFinding? value)
    {
        if (value is not null && value != JJDisputedCountRoPFinding.UNKNOWN)
        {
            return value.Value.ToString();
        }

        return string.Empty;
    }

    private string ToString(JJDisputedCountAppearInCourt? value)
    {
        if (value is not null && value != JJDisputedCountAppearInCourt.UNKNOWN)
        {
            switch (value)
            {
                case JJDisputedCountAppearInCourt.Y: return "I want to appear in court";
                case JJDisputedCountAppearInCourt.N: return "I do not want to appear in court";
            }
        }

        return string.Empty;
    }

    private string ToString(JJDisputedCountIncludesSurcharge? value)
    {
        if (value is not null && value != JJDisputedCountIncludesSurcharge.UNKNOWN)
        {
            return value.Value.ToString();
        }

        return JJDisputedCountIncludesSurcharge.N.ToString();
    }

    private string ToString(JJDisputedCountRoPJailIntermittent? value)
    {
        if (value is not null && value != JJDisputedCountRoPJailIntermittent.UNKNOWN)
        {
            return value.Value.ToString();
        }

        return string.Empty;
    }

    private string ToString(JJDisputedCountRoPDismissed? value)
    {
        if (value is not null && value != JJDisputedCountRoPDismissed.UNKNOWN)
        {
            return value.Value.ToString();
        }

        return string.Empty;
    }

    private string ToString(JJDisputedCountRoPForWantOfProsecution? value)
    {
        if (value is not null && value != JJDisputedCountRoPForWantOfProsecution.UNKNOWN)
        {
            return value.Value.ToString();
        }

        return string.Empty;
    }

    private string ToString(JJDisputedCountRoPWithdrawn? value)
    {
        if (value is not null && value != JJDisputedCountRoPWithdrawn.UNKNOWN)
        {
            return value.Value.ToString();
        }

        return string.Empty;
    }

    private string ToString(JJDisputedCountRoPAbatement? value)
    {
        if (value is not null && value != JJDisputedCountRoPAbatement.UNKNOWN)
        {
            return value.Value.ToString();
        }

        return string.Empty;
    }

    private string FormatAddress(JJDispute dispute)
    {
        // Filter out null or empty strings before joining
        var addressParts = new[] { dispute.AddressLine1, dispute.AddressLine2, dispute.AddressLine3, dispute.AddressCity, dispute.AddressProvince, dispute.AddressCountry, dispute.AddressPostalCode };
        var nonEmptyParts = addressParts.Where(part => !string.IsNullOrEmpty(part));
        // Concatenates all the address fields by a comma and returns them as a single string
        return string.Join(", ", nonEmptyParts);
    }

    private string ConcatenateWithSpaces(params string[] values)
    {
        if (values is null || values.Length == 0)
        {
            return string.Empty;
        }

        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < values.Length; i++)
        {
            if (values[i] is null)
            {
                continue;
            }

            if (builder.Length > 0)
            {
                builder.Append(' ');
            }

            builder.Append(values[i]);
        }

        return builder.ToString();
    }

    private bool SetReviseFine(JJDisputedCount disputedCount)
    {
        return !(disputedCount.LesserOrGreaterAmount is null || disputedCount.LesserOrGreaterAmount == 0);
    }

    private bool IsDueDateRevised(JJDisputedCount disputedCount)
    {
        return !(disputedCount.RevisedDueDate is null || disputedCount.RevisedDueDate == disputedCount.DueDate);
    }
}
