using MassTransit;
using Microsoft.Extensions.Options;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Staff.Service.Configuration;
using TrafficCourts.Staff.Service.Mappers;
using TrafficCourts.Staff.Service.OpenAPIs.OracleDataApi.v1_0;

namespace TrafficCourts.Staff.Service.Services;

/// <summary>
/// Summary description for Class1
/// </summary>
public class DisputeService : IDisputeService
{
    private readonly OracleDataApiConfiguration _oracleDataApiConfiguration;
    private readonly ILogger<DisputeService> _logger;
    private readonly IBus _bus;

    public DisputeService(OracleDataApiConfiguration oracleDataApiConfiguration, IBus bus, ILogger<DisputeService> logger)
    {
        _oracleDataApiConfiguration = oracleDataApiConfiguration ?? throw new ArgumentNullException(nameof(oracleDataApiConfiguration));
        _bus = bus ?? throw new ArgumentNullException(nameof(bus));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Returns a new inialized instance of the OracleDataApi_v1_0Client
    /// </summary>
    /// <returns></returns>
    private OracleDataApi_v1_0Client GetOracleDataApi()
    {
        OracleDataApi_v1_0Client client = new(new HttpClient());
        client.BaseUrl = _oracleDataApiConfiguration.BaseUrl;
        return client;
    }

    public async Task<ICollection<Dispute>> GetAllDisputesAsync(CancellationToken cancellationToken)
    {
        return await GetOracleDataApi().GetAllDisputesAsync(cancellationToken);
    }

    public async Task<Guid> SaveDisputeAsync(Dispute dispute, CancellationToken cancellationToken)
    {
        return await GetOracleDataApi().SaveDisputeAsync(dispute, cancellationToken);
    }

    public async Task<Dispute> GetDisputeAsync(Guid disputeId, CancellationToken cancellationToken)
    {
        return await GetOracleDataApi().GetDisputeAsync(disputeId, cancellationToken);
    }

    public async Task<Dispute> UpdateDisputeAsync(Guid disputeId, Dispute dispute, System.Threading.CancellationToken cancellationToken)
    {
        return await GetOracleDataApi().UpdateDisputeAsync(disputeId, dispute, cancellationToken);
    }

    public async Task CancelDisputeAsync(Guid disputeId, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Dispute cancelled");

        Dispute dispute = await GetOracleDataApi().CancelDisputeAsync(disputeId, cancellationToken);

        // Publish submit event (consumer(s) will generate email, etc)
        DisputeCancelled cancelledEvent = Mapper.ToDisputeCancelled(dispute);
        await _bus.Publish(cancelledEvent, cancellationToken);

        SendEmail cancelSendEmail = Mapper.ToCancelSendEmail(dispute);
        await _bus.Publish(cancelSendEmail, cancellationToken);
    }

    public async Task RejectDisputeAsync(Guid disputeId, string rejectedReason, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Dispute rejected");

        Dispute dispute = await GetOracleDataApi().RejectDisputeAsync(disputeId, rejectedReason, cancellationToken);

        // Publish submit event (consumer(s) will generate email, etc)
        DisputeRejected rejectedEvent = Mapper.ToDisputeRejected(dispute);
        await _bus.Publish(rejectedEvent, cancellationToken);

        SendEmail rejectSendEmail = Mapper.ToRejectSendEmail(dispute);
        await _bus.Publish(rejectSendEmail, cancellationToken);

    }

    public async Task SubmitDisputeAsync(Guid disputeId, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Dispute submitted for approval processing");

        // Save and status to PROCESSING
        Dispute dispute = await GetOracleDataApi().SubmitDisputeAsync(disputeId, cancellationToken);

        // Publish submit event (consumer(s) will push event to ARC and generate email)
        DisputeApproved approvedEvent = Mapper.ToDisputeApproved(dispute);
        await _bus.Publish(approvedEvent, cancellationToken);

        SendEmail processingSendEmail = Mapper.ToProcessingSendEmail(dispute);
        await _bus.Publish(processingSendEmail, cancellationToken);
    }

    public async Task DeleteDisputeAsync(Guid disputeId, CancellationToken cancellationToken)
    {
        await GetOracleDataApi().DeleteDisputeAsync(disputeId, cancellationToken);
    }
}
