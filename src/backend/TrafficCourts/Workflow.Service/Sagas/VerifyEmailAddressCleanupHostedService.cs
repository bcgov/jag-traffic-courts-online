using MassTransit;
using Microsoft.EntityFrameworkCore;
using TrafficCourts.Messaging.MessageContracts;

namespace TrafficCourts.Workflow.Service.Sagas;

/// <summary>
/// Temporary hosted service to re-raise the EmailVerificationSuccessful for all the verified email addresses.
/// </summary>
public partial class VerifyEmailAddressCleanupHostedService : IHostedService
{
    private readonly VerifyEmailAddressStateDbContext _context;
    private readonly IBus _bus;
    private readonly ILogger<VerifyEmailAddressCleanupHostedService> _logger;

    public VerifyEmailAddressCleanupHostedService(VerifyEmailAddressStateDbContext context, IBus bus, ILogger<VerifyEmailAddressCleanupHostedService> logger)
    {
        _context = context;
        _bus = bus;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        Starting();

        // oracle data api can take a couple of minutes to startup, so delay this processing
        // otherwise the consumer will get connection errors
        await Task.Delay(TimeSpan.FromMinutes(5), cancellationToken);

        try
        {
            var messages = await _context
                .Set<VerifyEmailAddressState>()
                .AsNoTracking()
                .Where(_ => _.VerifiedAt != null && _.TicketNumber != null && _.EmailAddress != null)
                .OrderBy(_ => _.VerifiedAt)
                .Select(_ => new EmailVerificationSuccessful
                {
                    NoticeOfDisputeGuid = _.NoticeOfDisputeGuid,
                    TicketNumber = _.TicketNumber!,
                    EmailAddress = _.EmailAddress!,
                    VerifiedAt = _.VerifiedAt!.Value,
                    IsUpdateEmailVerification = _.IsUpdateEmailVerification
                })
                .ToListAsync(cancellationToken);

            foreach (var message in messages)
            {
                await _bus.Publish(message, cancellationToken);
            }
        }
        catch (Exception exception)
        {
            Failed(exception);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Stopping();
        return Task.CompletedTask;
    }

    [LoggerMessage(Level = LogLevel.Debug, Message = "Starting")]
    private partial void Starting();

    [LoggerMessage(Level = LogLevel.Debug, Message = "Stopping")]
    private partial void Stopping();

    [LoggerMessage(Level = LogLevel.Debug, Message = "Processing processing verified email addresses failed")]
    private partial void Failed(Exception exception);

}
