using MassTransit;
using Microsoft.Extensions.Logging;
using TrafficCourts.Domain.Models;
using TrafficCourts.Interfaces;

namespace TrafficCourts.Common.Features.DisputeCreation;

public class DisputeCreationService(
    ILogger<DisputeCreationService> logger,
    IOracleDataApiService oracleDataApiService,
    IBus bus)
    : IDisputeCreationService
{
    /// <inheritdoc/>
    public async Task<bool> CanCreateDispute(string ticketNumber, CancellationToken cancellationToken)
    {
        // retrieve existing disputes for this ticket that haven't been rejected
        IList<DisputeResult> existingDisputes = await oracleDataApiService.SearchDisputeAsync(
            ticketNumber,
            null,
            null,
            ExcludeStatus2.REJECTED,
            cancellationToken);

        // if any of the existing disputes have a valid NoticeOfDisputeGuid, a new dispute cannot be created
        return !existingDisputes.Any(d => Guid.TryParse(d.NoticeOfDisputeGuid, out _));
    }
}
