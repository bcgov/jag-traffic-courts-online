using MassTransit;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Workflow.Service.Services;
using TrafficCourts.Workflow.Service.Models;

namespace TrafficCourts.Workflow.Service.Consumers
{
    /// <summary>
    ///     Consumer for DisputeApproved message.
    /// </summary>
    public class DisputeApprovedConsumer : IConsumer<DisputeApproved>
    {
        private readonly ILogger<DisputeApprovedConsumer> _logger;
        private readonly ISubmitDisputeToArcService _submitDisputeToArcService;

        public DisputeApprovedConsumer(ILogger<DisputeApprovedConsumer> logger, ISubmitDisputeToArcService submitDisputeToArcService)
        {
            _logger = logger;
            _submitDisputeToArcService = submitDisputeToArcService;
        }
        public async Task Consume(ConsumeContext<DisputeApproved> context)
        {
            try
            {
                if (context.MessageId != null)
                {
                    _logger.LogDebug("Consuming message: {MessageId} ", context.MessageId);
                }

                List<Models.TicketCount> ticketDetails = new();

                foreach (var vtc in context.Message.ViolationTicketCounts)
                {
                    var ticketDetail = new Models.TicketCount
                    {
                        Count = vtc.Count,
                        Section = vtc.Section,
                        Act = vtc.Act,
                        Amount = vtc.Amount
                    };
                    ticketDetails.Add(ticketDetail);
                }

                // Add dispute details model as part of TcoDisputeTicket model if defined in the message
                List<DisputeDetail> disputeDetails = new();

                if (context.Message.DisputeCounts != null && context.Message.DisputeCounts.Any())
                {
                    foreach (Messaging.MessageContracts.DisputeCount dc in context.Message.DisputeCounts)
                    {
                        Models.DisputeDetail disputeDetail = new Models.DisputeDetail
                        {
                            Count = dc.Count,
                            DisputeType = dc.DisputeType
                        };

                        disputeDetails.Add(disputeDetail);
                    }
                }

                var tcoDisputeTicket = new TcoDisputeTicket
                {
                    CitizenName = context.Message.CitizenName,
                    TicketIssuanceDate = context.Message.TicketIssuanceDate,
                    TicketFileNumber = context.Message.TicketFileNumber,
                    IssuingOrganization = context.Message.IssuingOrganization,
                    IssuingLocation = context.Message.IssuingLocation,
                    DriversLicence = context.Message.DriversLicence,
                    TicketDetails = ticketDetails,
                    StreetAddress = context.Message.StreetAddress,
                    City = context.Message.City,
                    Province = context.Message.Province,
                    PostalCode = context.Message.PostalCode,
                    Email = context.Message.Email,
                    DisputeDetails = disputeDetails
                };

                _logger.LogDebug("TRY SENDING APPROVED DISPUTE TO ARC: {ApprovedDisputeTicket} ", tcoDisputeTicket.ToString());

                await _submitDisputeToArcService.SubmitDisputeToArcAsync(tcoDisputeTicket);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error: ", ex);
            }
        }
    }
}
