using AutoMapper;
using MassTransit;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Workflow.Service.Services;

namespace TrafficCourts.Workflow.Service.Consumers
{
    /// <summary>
    ///     Consumer for SubmitDispute message.
    /// </summary>
    public class DisputeSubmitConsumer : IConsumer<SubmitNoticeOfDispute>
    {
        private readonly ILogger<DisputeSubmitConsumer> _logger;
        private readonly IOracleDataApiService _oracleDataApiService;
        private readonly IMapper _mapper;
        private readonly IBus _bus;
        private readonly IEmailSenderService _emailSenderService;

        public DisputeSubmitConsumer(ILogger<DisputeSubmitConsumer> logger, IOracleDataApiService oracleDataApiService, IMapper mapper, IBus bus, IEmailSenderService emailSenderService)
        {
            ArgumentNullException.ThrowIfNull(bus);
            _logger = logger;
            _oracleDataApiService = oracleDataApiService;
            _mapper = mapper;
            _bus = bus;
            _emailSenderService = emailSenderService;   
        }

        public async Task Consume(ConsumeContext<SubmitNoticeOfDispute> context)
        {
            using var messageIdScope = _logger.BeginScope(new Dictionary<string, object> { 
                { "MessageId", context.MessageId! }, 
                { "MessageType", nameof(SubmitNoticeOfDispute) } 
            });

            try
            {
                _logger.LogDebug("Consuming message");

                Dispute dispute = _mapper.Map<Dispute>(context.Message);
                string host = context.Message.Host; // part of link in verification email

                _logger.LogTrace("TRY CREATING DISPUTE: {@Dispute}", dispute);

                var disputeId = await _oracleDataApiService.CreateDisputeAsync(dispute);

                if (disputeId > 0)
                {
                    _logger.LogDebug("Dispute has been saved with {DisputeId}: ", disputeId);

                    // TCVP-1529 Saving a dispute should also send a verification email to the Disputant.
                    SendEmail sendEmail = _emailSenderService.ToVerificationSendEmail(dispute, host);
                    await _bus.Publish(sendEmail);

                    await context.RespondAsync<DisputeSubmitted>(new
                    {
                        context.MessageId,
                        InVar.Timestamp,
                        DisputeId = disputeId
                    });
                }
                else
                {
                    _logger.LogDebug("Failed to save the dispute");

                    await context.RespondAsync<DisputeRejected>(new
                    {
                        context.MessageId,
                        InVar.Timestamp,
                        Reason = "Bad request"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process message");
                throw;
            }
        }
    }
}
