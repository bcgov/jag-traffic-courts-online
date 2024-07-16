using TrafficCourts.Cdogs.Client;

namespace TrafficCourts.Staff.Service.Services;

public interface IPrintDigitalCaseFileService
{
    /// <summary>
    /// Renders the digital case file for a given dispute based on ticket number. This really should be using the tco_dispute.dispute_id.
    /// </summary>
    Task<RenderedReport> PrintDigitalCaseFileAsync(string ticketNumber, TimeZoneInfo timeZone, CancellationToken cancellationToken);

    /// <summary>
    /// Renders the ticket validation view for a given OCCAM dispute data based on dispute id.
    /// </summary>
    /// <param name="disputeId"></param>
    /// <param name="timeZone"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<RenderedReport> PrintTicketValidationViewAsync(long disputeId, TimeZoneInfo timeZone, CancellationToken cancellationToken);
}
