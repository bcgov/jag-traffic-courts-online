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
            using var messageIdScope = _logger.BeginScope(new Dictionary<string, object> { 
                { "MessageId", context.MessageId! }, 
                { "MessageType", nameof(DisputeApproved) } 
            });

            try
            {
                _logger.LogDebug("Consuming message");

                List<Models.TicketCount> ticketDetails = new();

                foreach (var vtc in context.Message.ViolationTicketCounts)
                {
                    var ticketDetail = new Models.TicketCount
                    {
                        Count = vtc.Count,
                        FullSection = vtc.FullSection,
                        Section = vtc.Section,
                        Subsection = vtc.Subsection,
                        Paragraph = vtc.Paragraph,
                        Act = vtc.Act,
                        Amount = vtc.Amount
                    };
                    ticketDetails.Add(ticketDetail);
                }

                // Add dispute details model as part of TcoDisputeTicket model if defined in the message
                List<Models.DisputeCount> disputeCounts = new();

                if (context.Message.DisputeCounts != null && context.Message.DisputeCounts.Any())
                {
                    foreach (Messaging.MessageContracts.DisputeCount dc in context.Message.DisputeCounts)
                    {
                        Models.DisputeCount disputeDetail = new Models.DisputeCount
                        {
                            Count = dc.Count,
                            DisputeType = dc.DisputeType
                        };

                        disputeCounts.Add(disputeDetail);
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
                    DisputeCounts = disputeCounts
                };

                // use trace because we are logging PII
                _logger.LogTrace("TRY SENDING APPROVED DISPUTE TO ARC: {ApprovedDisputeTicket} ", tcoDisputeTicket);

                await _submitDisputeToArcService.SubmitDisputeToArcAsync(tcoDisputeTicket);
            }
            catch (Services.ApiException ex)
            {
                _logger.LogError(ex, "ARC API request has failed to create an ARC file");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process message");
                throw;
            }
        }
    }
}
