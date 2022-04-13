using MassTransit;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Workflow.Service.Models;
using TrafficCourts.Workflow.Service.Services;
using TicketCount = TrafficCourts.Workflow.Service.Models.TicketCount;

namespace TrafficCourts.Workflow.Service.Consumers
{
    /// <summary>
    ///     Consumer for SubmitDispute message.
    /// </summary>
    public class SubmitDisputeConsumer : IConsumer<SubmitDispute>
    {
        private readonly ILogger<SubmitDisputeConsumer> _logger;
        private readonly IOracleDataApiService _oracleDataApiService;

        public SubmitDisputeConsumer(ILogger<SubmitDisputeConsumer> logger, IOracleDataApiService oracleDataApiService)
        {
            _logger = logger;
            _oracleDataApiService = oracleDataApiService;
        }

        public async Task Consume(ConsumeContext<SubmitDispute> context)
        {
            if (context.RequestId != null)
            {
                _logger.LogDebug("Consuming message: {MessageId}", context.MessageId);

                List<TicketCount> ticketCounts = new();

                foreach (var ticketCount in context.Message.TicketCounts)
                {
                    var ticket = new TicketCount
                    {
                        FineReductionRequest = ticketCount.FineReductionRequest,
                        OffenceDeclaration = ticketCount.OffenceDeclaration,
                        TimeToPayRequest = ticketCount.TimeToPayRequest,
                    };
                    ticketCounts.Add(ticket);
                }

                var dispute = new Dispute
                {
                    TicketNumber = context.Message.TicketNumber,
                    CourtLocation = context.Message.CourtLocation,
                    ViolationDate = context.Message.ViolationDate,
                    DisputantSurname = context.Message.DisputantSurname,
                    GivenNames = context.Message.GivenNames,
                    StreetAddress = context.Message.StreetAddress,
                    Province = context.Message.Province,
                    PostalCode = context.Message.PostalCode,
                    HomePhone = context.Message.HomePhone,
                    DriversLicence = context.Message.DriversLicence,
                    DriversLicenceProvince = context.Message.DriversLicenceProvince,
                    WorkPhone = context.Message.WorkPhone,
                    DateOfBirth = context.Message.DateOfBirth,
                    EnforcementOrganization = context.Message.EnforcementOrganization,
                    ServiceDate = context.Message.ServiceDate,
                    TicketCounts = ticketCounts,
                    LawyerRepresentation = context.Message.LawyerRepresentation,
                    InterpreterLanguage = context.Message.InterpreterLanguage,
                    WitnessIntent = context.Message.WitnessIntent
                };

                _logger.LogDebug("TRY CREATING DISPUTE: {Dispute}", dispute.ToString());

                var disputeId = await _oracleDataApiService.CreateDisputeAsync(dispute);

                if (disputeId != -1)
                {
                    _logger.LogDebug("Dispute has been saved with {DisputeId}: ", disputeId);

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
        }
    }
}
