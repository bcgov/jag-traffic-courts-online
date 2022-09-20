using MassTransit;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Workflow.Service.Services;
using ApiException = TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.ApiException;

namespace TrafficCourts.Workflow.Service.Consumers;

/// <summary>
/// Consumer for EmailSendVerification message, produced when a Disputant initially submits a Dispute or by manually triggering an email resend.
/// </summary>
public class EmailVerificationComposeConsumer : IConsumer<EmailVerificationSend>
{
    private readonly ILogger<EmailVerificationComposeConsumer> _logger;
    private readonly IOracleDataApiService _oracleDataApiService;
    private readonly IBus _bus;
    private readonly IEmailSenderService _emailSenderService;

    public EmailVerificationComposeConsumer(ILogger<EmailVerificationComposeConsumer> logger, IOracleDataApiService oracleDataApiService, IBus bus, IEmailSenderService emailSenderService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        ArgumentNullException.ThrowIfNull(oracleDataApiService);
        _oracleDataApiService = (oracleDataApiService);
        ArgumentNullException.ThrowIfNull(bus);
        _emailSenderService = (emailSenderService);
        ArgumentNullException.ThrowIfNull(emailSenderService);
        _bus = bus;
    }

    public async Task Consume(ConsumeContext<EmailVerificationSend> context)
    {
        using var messageIdScope = _logger.BeginScope(new Dictionary<string, object> {
                { "MessageId", context.MessageId! },
                { "MessageType", nameof(EmailVerificationSend) }
            });
        EmailVerificationSend message = context.Message;
        try
        {
            Dispute dispute = await _oracleDataApiService.GetDisputeByEmailVerificationTokenAsync(message.EmailVerificationToken.ToString());
            SendEmail sendEmail = _emailSenderService.ToVerificationEmail(dispute, context.Message.Host);
            await _bus.Publish(sendEmail);
        }
        catch (ApiException ex) when (ex.StatusCode == StatusCodes.Status404NotFound)
        {
            // This occurs if a Dispute is already verified since the EmailVerificationToken is cleared on the record thus no record is returned.
            _logger.LogError(ex, "Failed to retrieve Dispute by EmailVerificationToken");
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "Failed to retrieve Dispute by EmailVerificationToken");
            throw;
        }
    }
}
