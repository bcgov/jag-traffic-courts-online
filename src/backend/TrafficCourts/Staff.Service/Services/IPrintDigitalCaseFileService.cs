﻿using TrafficCourts.Cdogs.Client;

namespace TrafficCourts.Staff.Service.Services;

public interface IPrintDigitalCaseFileService
{
    /// <summary>
    /// Renders the digital case file for a given dispute based on ticket number. This really should be using the tco_dispute.dispute_id.
    /// </summary>
    Task<RenderedReport> PrintDigitalCaseFileAsync(string ticketNumber, string timeZoneId, CancellationToken cancellationToken);
}
