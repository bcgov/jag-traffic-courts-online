using AutoMapper;
using MassTransit;
using TrafficCourts.Common.Features.Mail.Templates;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Workflow.Service.Services;

namespace TrafficCourts.Workflow.Service.Consumers;

/// <summary>
///     Consumer for SubmitDispute message.
/// </summary>
public class DisputeSubmitConsumer : IConsumer<SubmitNoticeOfDispute>
{
    private readonly ILogger<DisputeSubmitConsumer> _logger;
    private readonly IOracleDataApiService _oracleDataApiService;
    private readonly IMapper _mapper;
    private readonly IVerificationEmailTemplate _verificationEmailTemplate;

    public DisputeSubmitConsumer(ILogger<DisputeSubmitConsumer> logger, IOracleDataApiService oracleDataApiService, IMapper mapper, IVerificationEmailTemplate verificationEmailTemplate)
    {
        _logger = logger;
        _oracleDataApiService = oracleDataApiService;
        _mapper = mapper;
        _verificationEmailTemplate = verificationEmailTemplate ?? throw new ArgumentNullException(nameof(verificationEmailTemplate));
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

            _logger.LogTrace("TRY CREATING DISPUTE: {@Dispute}", dispute);

            var disputeId = await _oracleDataApiService.CreateDisputeAsync(dispute);

            if (disputeId > 0 && (dispute.EmailAddress is not null && dispute.EmailAddress.Trim() != "") && dispute.EmailAddressVerified == false)
            {
                _logger.LogDebug("Dispute has been saved with {DisputeId}: ", disputeId);

                // File History
                SaveFileHistoryRecord fileHistoryRecord = new SaveFileHistoryRecord();
                fileHistoryRecord.TicketNumber = dispute.TicketNumber;
                fileHistoryRecord.Description = "Dispute initiated.";
                await context.Publish(fileHistoryRecord);

                // TCVP-1529 Saving a dispute should also send a verification email to the Disputant if email address is present.
                var emailMessage = _verificationEmailTemplate.Create(dispute);
                await context.Publish(new SendDispuantEmail
                {
                    Message = emailMessage,
                    TicketNumber = dispute.TicketNumber,
                    NoticeOfDisputeId = context.Message.NoticeOfDisputeId
                }, context.CancellationToken);

                await context.RespondAsync<DisputeSubmitted>(new
                {
                    context.MessageId,
                    InVar.Timestamp,
                    DisputeId = disputeId
                });
            }
            else if (disputeId > 0 && (dispute.EmailAddress is null || dispute.EmailAddress.Trim() == ""))
            {
                _logger.LogDebug("Dispute has been saved with {DisputeId}: ", disputeId);

                // File History
                SaveFileHistoryRecord fileHistoryRecord = new SaveFileHistoryRecord();
                fileHistoryRecord.TicketNumber = dispute.TicketNumber;
                fileHistoryRecord.Description = "Dispute initiated.";
                await context.Publish(fileHistoryRecord, context.CancellationToken);

                // File History
                fileHistoryRecord.Description = "Dispute submitted for staff review.";
                await context.Publish(fileHistoryRecord, context.CancellationToken);

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
