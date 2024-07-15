using TrafficCourts.Cdogs.Client;
using TrafficCourts.Staff.Service.Models.DigitalCaseFiles.Print;

namespace TrafficCourts.Staff.Service.Services;

public interface IPrintDigitalCaseFileService
{
    /// <summary>
    /// Renders the digital case file for a given dispute based on ticket number. This really should be using the tco_dispute.dispute_id.
    /// </summary>
    Task<RenderedReport> PrintDigitalCaseFileAsync(string ticketNumber, string timeZoneId, DcfTemplateType type, CancellationToken cancellationToken);
    Task<RenderedReport> PrintDigitalCaseFileAsync(string ticketNumber, TimeZoneInfo timeZone, DcfTemplateType type, CancellationToken cancellationToken);

    /// <summary>
    /// Renders the ticket validation view for a given OCCAM dispute data based on dispute id.
    /// </summary>
    /// <param name="disputeId"></param>
    /// <param name="timeZoneId"></param>
    /// <param name="type"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<RenderedReport> PrintTicketValidationViewAsync(long disputeId, TimeZoneInfo timeZone, DcfTemplateType type, CancellationToken cancellationToken);
}
