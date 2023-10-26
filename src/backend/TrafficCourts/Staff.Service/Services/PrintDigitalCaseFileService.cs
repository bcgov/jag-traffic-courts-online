using Microsoft.Extensions.FileProviders;
using NodaTime;
using System.Text;
using TrafficCourts.Cdogs.Client;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Staff.Service.Models.DigitalCaseFiles.Print;

namespace TrafficCourts.Staff.Service.Services;

public interface IPrintDigitalCaseFileService
{
    Task<RenderedReport> PrintDigitalCaseFileAsync(long disputeId, string timeZoneId, CancellationToken cancellationToken);
}

public class PrintDigitalCaseFileService : IPrintDigitalCaseFileService
{
    private readonly IJJDisputeService _disputeService;
    private readonly IDateTimeZoneProvider _timeZoneProvider;
    private readonly IDocumentGenerationService _documentGeneration;
    private readonly ILogger<PrintDigitalCaseFileService> _logger;

    public PrintDigitalCaseFileService(
        IJJDisputeService disputeService, 
        IDateTimeZoneProvider timeZoneProvider, 
        IDocumentGenerationService documentGeneration,
        ILogger<PrintDigitalCaseFileService> logger)
    {
        _disputeService = disputeService ?? throw new ArgumentNullException(nameof(disputeService));
        _timeZoneProvider = timeZoneProvider ?? throw new ArgumentNullException(nameof(timeZoneProvider));
        _documentGeneration = documentGeneration ?? throw new ArgumentNullException(nameof(documentGeneration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<RenderedReport> PrintDigitalCaseFileAsync(long disputeId, string timeZoneId, CancellationToken cancellationToken)
    {
        // generate the digital case file model
        DigitalCaseFile digitalCaseFile = await GetDigitalCaseFileAsync(disputeId, timeZoneId, cancellationToken);

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

    private async Task<DigitalCaseFile> GetDigitalCaseFileAsync(long disputeId, string timeZoneId, CancellationToken cancellationToken)
    {
        // JavaScript: Intl.DateTimeFormat().resolvedOptions().timeZone
        // Time Zone from the browser is either a time zone identifier from the IANA Time Zone Database or a UTC offset in ISO 8601 extended format.
        // https://tc39.es/ecma402/#sec-properties-of-intl-datetimeformat-instances

        // get the user's time zone 
        DateTimeZone? timeZone = _timeZoneProvider[timeZoneId]; // can throw DateTimeZoneNotFoundException.

        var dispute = await _disputeService.GetJJDisputeAsync(disputeId, null, false, cancellationToken);

        var digitalCaseFile = new DigitalCaseFile();

        // fill in each section, the sections and fields are populated in order matching the template

        // set the ticket information
        var ticket = digitalCaseFile.Ticket;
        ticket.Surname = dispute.OccamDisputantSurnameNm;
        ticket.GivenNames = ConcatenateWithSpaces(dispute.OccamDisputantGiven1Nm, dispute.OccamDisputantGiven2Nm, dispute.OccamDisputantGiven3Nm);
        ticket.OffenceLocation = dispute.OffenceLocation;
        ticket.PoliceDetachment = dispute.PoliceDetachment;
        ticket.IssuedDate = ToDateTime(dispute.IssuedTs); // DateTimeOffset > DateTime
        ticket.SubmittedDate = ToDateTime(dispute.SubmittedTs); // DateTimeOffset > DateTime
        ticket.IcbcReceivedDate = ToDateTime(dispute.IcbcReceivedDate); // DateTimeOffset > DateTime
        ticket.CourtAgenyId = dispute.CourtAgenId;
        ticket.CourtHouse = dispute.CourthouseLocation;

        // set the contact information
        var contact = digitalCaseFile.Contact;
        contact.Surname = dispute.ContactSurname;
        contact.GivenNames = ConcatenateWithSpaces(dispute.ContactGivenName1, dispute.ContactGivenName2, dispute.ContactGivenName3);
        contact.Address = FormatAddress(dispute);
        contact.DriversLicence.Province = dispute.DrvLicIssuedProvSeqNo + " ** ProvSeqNo -> code"; // this is not correct
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
        // TODO: how do we know the current appearance vs historical ones?
        JJDisputeCourtAppearanceRoP currentAppearance = dispute.JjDisputeCourtAppearanceRoPs.First();
        SetFields(appearance, currentAppearance);

        // set the court appearance history
        var appearanceHistory = digitalCaseFile.AppearanceHistory;
        foreach (var rop in dispute.JjDisputeCourtAppearanceRoPs)
        {
            // TODO: how do we know the current appearance vs historical ones?
            appearanceHistory.Add(SetFields(new Appearance(), rop));
        }

        // set written reasons
        var writtenReasons = digitalCaseFile.WrittenReasons;
        writtenReasons.FineReduction = dispute.FineReductionReason;
        writtenReasons.TimeToPay = dispute.TimeToPayReason;

        // set the counts
        var counts = digitalCaseFile.Counts;
        foreach (var disputedCount in dispute.JjDisputedCounts.OrderBy(_ => _.Count))
        {
            counts.Add(new OffenseCount
            {
                Count = disputedCount.Count,
                Plea = ToString(disputedCount.Plea),
                Description = disputedCount.Description,
                Due = ToDateTime(disputedCount.DueDate),
                Fine = disputedCount.TicketedFineAmount != null ? (decimal)disputedCount.TicketedFineAmount : 0m,
                //Request = ToString(disputedCount.RequestReduction, disputedCount.RequestTimeToPay),
                RequestFineReduction = ToString(disputedCount.RequestReduction),
                RequestTimeToPay = ToString(disputedCount.RequestTimeToPay),
            });
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

        // TODO: get file history

        // set file remarks
        var remarks = digitalCaseFile.FileRemarks;
        foreach (var remark in dispute.Remarks)
        {
            remarks.Add(new FileRemark
            {
                Date = ToDateTime(remark.RemarksMadeTs),
                Username = remark.UserFullName,
                Note = remark.Note,
            });
        }

        return digitalCaseFile;
    }



    private Appearance SetFields(Appearance appearance, JJDisputeCourtAppearanceRoP appearanceRop)
    {
        appearance.Date = ToDateTime(appearanceRop.AppearanceTs);
        appearance.Room = appearanceRop.Room;
        appearance.App = ToString(appearanceRop.AppCd);
        appearance.NoApp = ToDateTime(appearanceRop.NoAppTs);
        appearance.DefenseCouncil = appearanceRop.DefenceCounsel;
        appearance.DefenseAtt = ToString(appearanceRop.DattCd);
        appearance.Crown = ToString(appearanceRop.Crown);
        appearance.Seized = ToString(appearanceRop.JjSeized);
        appearance.JudicialJustice = appearanceRop.Adjudicator;
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
            return value.ToString();
        }

        return string.Empty;
    }
    private string ToString(JJDisputeElectronicTicketYn? value)
    {
        if (value is not null && value != JJDisputeElectronicTicketYn.UNKNOWN)
        {
            return value.ToString();
        }

        return string.Empty;
    }
    private string ToString(JJDisputeCourtAppearanceRoPJjSeized? value)
    {
        if (value is not null && value != JJDisputeCourtAppearanceRoPJjSeized.UNKNOWN)
        {
            return value.ToString();
        }

        return string.Empty;
    }

    private string ToString(JJDisputeCourtAppearanceRoPCrown? value)
    {
        if (value is not null && value != JJDisputeCourtAppearanceRoPCrown.UNKNOWN)
        {
            return value.ToString();
        }

        return string.Empty;
    }
    private string ToString(JJDisputeCourtAppearanceRoPDattCd? value)
    {
        if (value is not null && value != JJDisputeCourtAppearanceRoPDattCd.UNKNOWN)
        {
            return value.ToString();
        }

        return string.Empty;
    }

    private string ToString(JJDisputeCourtAppearanceRoPAppCd? value)
    {
        if (value is not null && value != JJDisputeCourtAppearanceRoPAppCd.UNKNOWN)
        {
            return value.ToString(); 
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

    private DateTime ToDateTime(DateTimeOffset? value)
    {
        return DateTime.MinValue;
    }

    private string FormatAddress(JJDispute dispute)
    {
        StringBuilder builder = new StringBuilder();

        return builder.ToString();
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
}
