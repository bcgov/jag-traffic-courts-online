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
            if (context.RequestId != null)
            {
                _logger.LogInformation("DisputeApprovedConsumer is consuming message: " + context.Message.Id);

                List<Models.TicketDetails> ticketDetails = new List<Models.TicketDetails>();

                foreach (var td in context.Message.TicketDetails)
                {
                    var ticketDetail = new Models.TicketDetails
                    {
                        Section = td.Section,
                        Subsection = td.Subsection,
                        Paragraph = td.Paragraph,
                        Act = td.Act,
                        Amount = td.Amount
                    };
                    ticketDetails.Add(ticketDetail);
                }

                // Add dispute details model as part of TcoDisputeTicket model if defined in the message
                Dictionary<string, Models.DisputeDetails>[] disputeDetailsArray = new Dictionary<string, Models.DisputeDetails>[3];

                if (context.Message.DisputeDetails != null && context.Message.DisputeDetails.Length > 0 && context.Message.DisputeDetails.Length < 4)
                {
                    for (int i = 0; i < context.Message.DisputeDetails.Length; i++)
                    {
                        Dictionary<string, Models.DisputeDetails> disputeDetailsDictionary = new Dictionary<string, Models.DisputeDetails>();

                        foreach (KeyValuePair<string, Messaging.MessageContracts.DisputeDetails> kvp in context.Message.DisputeDetails[i])
                        {
                            Models.DisputeDetails disputeDetail = new Models.DisputeDetails
                            {
                                DisputeType = kvp.Value.DisputeType,
                                DisputeReason = kvp.Value.DisputeReason
                            };

                            disputeDetailsDictionary.Add(kvp.Key, disputeDetail);
                        }

                        disputeDetailsArray[i] = disputeDetailsDictionary;
                    }
                }

                var tcoDisputeTicket = new TcoDisputeTicket
                {
                    CitizenName = context.Message.CitizenName,
                    TicketIssuanceDate = context.Message.TicketIssuanceDate,
                    TicketFileNumber = context.Message.TicketFileNumber,
                    IssuingOrganization = context.Message.IssuingOrganization,
                    IssuingLocation = context.Message.IssuingLocation,
                    DriversLicense = context.Message.DriversLicense,
                    TicketDetails = ticketDetails,
                    StreetAddress = context.Message.StreetAddress,
                    City = context.Message.City,
                    Province = context.Message.Province,
                    PostalCode = context.Message.PostalCode,
                    Email = context.Message.Email,
                    DisputeDetails = disputeDetailsArray
                };

                _logger.LogInformation("TRY SENDING APPROVED DISPUTE TO ARC: " + tcoDisputeTicket.ToString());

                await _submitDisputeToArcService.SubmitDisputeToArcAsync(tcoDisputeTicket);

            }
        }
    }
}
