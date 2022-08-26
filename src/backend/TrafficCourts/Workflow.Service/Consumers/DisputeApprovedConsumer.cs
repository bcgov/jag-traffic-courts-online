using MassTransit;
using TrafficCourts.Arc.Dispute.Client;
using TrafficCourts.Messaging.MessageContracts;

using TicketCount = TrafficCourts.Arc.Dispute.Client.TicketCount;
using DisputeCount = TrafficCourts.Arc.Dispute.Client.DisputeCount;

namespace TrafficCourts.Workflow.Service.Consumers
{
    /// <summary>
    /// Consumer for DisputeApproved message.
    /// </summary>
    public class DisputeApprovedConsumer : IConsumer<DisputeApproved>
    {
        private readonly ILogger<DisputeApprovedConsumer> _logger;
        private readonly IArcDisputeClient _arcDisputeClient;

        public DisputeApprovedConsumer(ILogger<DisputeApprovedConsumer> logger, IArcDisputeClient arcDisputeClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _arcDisputeClient = arcDisputeClient ?? throw new ArgumentNullException(nameof(arcDisputeClient));
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

                TcoDisputeTicket tcoDisputeTicket = CreateDisputeTicket(context.Message);

                // use trace because we are logging PII
                _logger.LogTrace("TRY SENDING APPROVED DISPUTE TO ARC: {ApprovedDisputeTicket} ", tcoDisputeTicket);

                await _arcDisputeClient.TcoDisputeTicketAsync(tcoDisputeTicket, context.CancellationToken);
            }
            catch (ApiException ex)
            {
                _logger.LogError(ex, "ARC API request has failed to create an ARC file");
                throw;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process message");
                throw;
            }
        }

        /// <summary>
        /// Creates the list of counts found on the ticket.
        /// </summary>
        private List<TicketCount> CreateTicketCounts(DisputeApproved message)
        {
            List<TicketCount> ticketDetails = new();

            foreach (var ticketCount in message.ViolationTicketCounts)
            {
                if (ticketCount.Amount is null)
                {
                    _logger.LogInformation("Count {count} does not have an amount, skipping this count", ticketCount.Count);
                }
                else
                {
                    var ticketDetail = new TicketCount
                    {
                        Count = ticketCount.Count,
                        Full_section = ticketCount.FullSection,
                        Section = ticketCount.Section,
                        Subsection = ticketCount.Subsection,
                        Paragraph = ticketCount.Paragraph,
                        Act = ticketCount.Act,
                        Amount = ticketCount.Amount.Value
                    };
                    ticketDetails.Add(ticketDetail);
                }
            }

            return ticketDetails;
        }

        /// <summary>
        /// Creates the list of counts found on the dispute.
        /// </summary>
        private List<DisputeCount> CreateDisputeCounts(DisputeApproved message)
        {
            // Add dispute details model as part of TcoDisputeTicket model if defined in the message
            List<DisputeCount> disputeCounts = new();

            if (message.DisputeCounts != null && message.DisputeCounts.Any())
            {
                foreach (Messaging.MessageContracts.DisputedCount dc in message.DisputeCounts)                    
                {
                    DisputeCount disputeDetail = new()
                    {
                        Count = dc.Count,
                        Dispute_type = dc.DisputeType
                    };
                }
            }

            return disputeCounts;
        }

        /// <summary>
        /// Creates the disputed ticket for submission to ARC.
        /// </summary>
        private TcoDisputeTicket CreateDisputeTicket(DisputeApproved message)
        {
            if (message.TicketIssuanceDate is null)
            {
                var exception = new InvalidOperationException("No ticket issuance date found on dispute");
                _logger.LogError(exception.Message);
                throw exception;
            }

            List<TicketCount> ticketDetails = CreateTicketCounts(message);
            List<DisputeCount> disputeCounts = CreateDisputeCounts(message);

            TcoDisputeTicket disputeTicket = new()
            {
                Citizen_name = message.CitizenName,
                Ticket_issuance_date = message.TicketIssuanceDate.Value,
                Ticket_file_number = message.TicketFileNumber,
                Issuing_organization = message.IssuingOrganization,
                Issuing_location = message.IssuingLocation,
                Drivers_licence = message.DriversLicence,
                Ticket_details = ticketDetails,
                Street_address = message.StreetAddress,
                City = message.City,
                Province = message.Province,
                Postal_code = message.PostalCode,
                Email = message.Email,
                Dispute_counts = disputeCounts
            };

            return disputeTicket;
        }
    }
}
