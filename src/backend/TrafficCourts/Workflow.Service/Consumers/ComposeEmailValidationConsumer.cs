using MassTransit;
using System.Threading;
using TrafficCourts.Arc.Dispute.Client;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Staff.Service.Mappers;
using TrafficCourts.Workflow.Service.Services;
using ApiException = TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.ApiException;

namespace TrafficCourts.Workflow.Service.Consumers;

/// <summary>
///     Consumer for SendEmail message.
/// </summary>
public class ComposeEmailValidationConsumer : IConsumer<EmailSendValidation>
{
    private readonly ILogger<ComposeEmailValidationConsumer> _logger;
    private readonly IOracleDataApiService _oracleDataApiService;
    private readonly IBus _bus;

    public ComposeEmailValidationConsumer(ILogger<ComposeEmailValidationConsumer> logger, IOracleDataApiService oracleDataApiService, IBus bus)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        ArgumentNullException.ThrowIfNull(oracleDataApiService);
        _oracleDataApiService = (oracleDataApiService);
        ArgumentNullException.ThrowIfNull(bus);
        _bus = bus;
    }

    public async Task Consume(ConsumeContext<EmailSendValidation> context)
    {
        using var messageIdScope = _logger.BeginScope(new Dictionary<string, object> {
                { "MessageId", context.MessageId! },
                { "MessageType", nameof(DisputeApproved) }
            });
        EmailSendValidation message = context.Message;
        try
        {
            Dispute dispute = await _oracleDataApiService.GetDisputeByEmailVerificationTokenAsync(message.EmailValidationToken.ToString());
            SendEmail sendEmail = Mapper.ToVerificationSendEmail(dispute, context.Message.Host);
            await _bus.Publish(sendEmail);
        }
        catch (ApiException ex) when (ex.StatusCode == StatusCodes.Status404NotFound)
        {
            // This occurs if a Dispute is already verified since the EmailValidationToken is cleared on the record thus no record is returned.
            _logger.LogError(ex, "Failed to retrieve Dispute by EmailValidationToken");
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "Failed to retrieve Dispute by EmailValidationToken");
            throw;
        }
    }
}
