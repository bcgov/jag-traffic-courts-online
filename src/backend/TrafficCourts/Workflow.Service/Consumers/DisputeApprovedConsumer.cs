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
    public partial class DisputeApprovedConsumer : IConsumer<DisputeApproved>
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
            LogConsuming(context);

            // check preconditions outside of the try/catch. If preconditions are not met
            // this call will log and throw an exception preventing bad data from being 
            // sent to the ARC API.
            CheckPreconditions(context); // TODO: move to publisher?

            try
            {
                TcoDisputeTicket tcoDisputeTicket = CreateDisputeTicket(context);
                await _arcDisputeClient.TcoDisputeTicketAsync(tcoDisputeTicket, context.CancellationToken);

                LogConsumed(context);
            }
            catch (Exception exception)
            {
                LogArcApiFailed(exception, exception, context);
                throw;
            }
        }

        /// <summary>
        /// Checks for preconditions before trying to send to ARC.
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="InvalidOperationException"></exception>
        private void CheckPreconditions(ConsumeContext<DisputeApproved> context)
        {
            var message = context.Message;

            if (message.TicketIssuanceDate is null)
            {
                LogNoTicketIssuedDate(context);
                throw new InvalidOperationException("No ticket issuance date found on dispute");
            }

            if (message.DisputeCounts is null || message.DisputeCounts.Count == 0)
            {
                LogNoTicketCounts(context);
                throw new InvalidOperationException("No ticket counts date found on dispute");
            }

            if (message.ViolationTicketCounts.All(_ => _.Amount is null))
            {
                LogNoTicketCountAmounts(context);
            }
        }

        /// <summary>
        /// Creates the disputed ticket for submission to ARC.
        /// </summary>
        private TcoDisputeTicket CreateDisputeTicket(ConsumeContext<DisputeApproved> context)
        {
            DisputeApproved message = context.Message;

            TcoDisputeTicket disputeTicket = new()
            {
                Given_name1 = message.GivenName1,
                Given_name2 = message.GivenName2,
                Given_name3 = message.GivenName3,
                Surname = message.Surname,
                Ticket_issuance_date = message.TicketIssuanceDate!.Value, // not null checked prior to calling this method
                Ticket_file_number = message.TicketFileNumber,
                Issuing_organization = message.IssuingOrganization,
                Issuing_location = message.IssuingLocation,
                Drivers_licence = message.DriversLicence,
                Ticket_details = CreateTicketCounts(context),
                Street_address = message.StreetAddress,
                City = message.City,
                Province = message.Province,
                Postal_code = message.PostalCode,
                Email = message.Email,
                Dispute_counts = CreateDisputeCounts(context)
            };

            return disputeTicket;
        }

        /// <summary>
        /// Creates the list of counts found on the ticket.
        /// </summary>
        private List<TicketCount> CreateTicketCounts(ConsumeContext<DisputeApproved> context)
        {
            DisputeApproved message = context.Message; 
            List<TicketCount> ticketDetails = new();

            foreach (var ticketCount in message.ViolationTicketCounts)
            {
                if (ticketCount.Amount is null)
                {
                    LogCountSkippedDueToNoAmount(context, ticketCount.Count);
                    continue;
                }

                var ticketDetail = new TicketCount
                {
                    Count = ticketCount.Count,
                    Act = MapAct(ticketCount.Act),
                    Section = ticketCount.Section,
                    Subsection = ticketCount.Subsection,
                    Paragraph = ticketCount.Paragraph,
                    Subparagraph = ticketCount.Subparagraph,
                    Amount = ticketCount.Amount.Value
                };
                ticketDetails.Add(ticketDetail);
            }

            return ticketDetails;
        }

        /// <summary>
        /// Maps the act required by ARC
        /// </summary>
        /// <remarks>See issue TCVP-2795</remarks>
        private static string MapAct(string act) => act == "MVAR" ? "MVR" : act;

        /// <summary>
        /// Creates the list of counts found on the dispute.
        /// </summary>
        private List<DisputeCount> CreateDisputeCounts(ConsumeContext<DisputeApproved> context)
        {
            DisputeApproved message = context.Message; 

            var disputeCounts = message.DisputeCounts
                .OrderBy(_ => _.Count)
                .Select(_ => new DisputeCount { Count = _.Count, Dispute_type = _.DisputeType })
                .ToList();

            return disputeCounts;
        }

        [LoggerMessage(EventId = 0, Level = LogLevel.Debug, Message = "Consuming message", EventName = "BeginConsume")]
        private partial void LogConsuming(
            [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordTags), OmitReferenceName = true)]
            ConsumeContext<DisputeApproved> context);

        [LoggerMessage(EventId = 1, Level = LogLevel.Debug, Message = "Message consumed", EventName = "EndConsume")]
        private partial void LogConsumed(
            [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordTags), OmitReferenceName = true)]
            ConsumeContext<DisputeApproved> context);

        [LoggerMessage(EventId = 2, Level = LogLevel.Warning, Message = "No ticket issuance date found on dispute, cannot submit to ARC.", EventName = "NoTicketIssuedDate")]
        private partial void LogNoTicketIssuedDate(
            [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordTags), OmitReferenceName = true)]
            ConsumeContext<DisputeApproved> context);

        [LoggerMessage(EventId = 3, Level = LogLevel.Warning, Message = "No counts found on dispute, cannot submit to ARC.", EventName = "NoDisputeCounts")]
        private partial void LogNoTicketCounts(
            [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordTags), OmitReferenceName = true)]
            ConsumeContext<DisputeApproved> context);

        [LoggerMessage(EventId = 4, Level = LogLevel.Information, Message = "None of ticket counts have an amount specified ", EventName = "NoTicketCountAmounts")]
        private partial void LogNoTicketCountAmounts(
            [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordTags), OmitReferenceName = true)]
            ConsumeContext<DisputeApproved> context);

        [LoggerMessage(EventId = 5, Level = LogLevel.Error, Message = "ARC API request has failed", EventName = "ArcApiFailed")]
        private partial void LogArcApiFailed(
            Exception exception,
            [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordTags), OmitReferenceName = true)]
            Exception reason, // have to duplicate the parameter due to source generator
            [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordTags), OmitReferenceName = true)]
            ConsumeContext<DisputeApproved> context);

        [LoggerMessage(EventId = 6, Level = LogLevel.Information, Message = "Ticket count skipped because there was no amount", EventName = "CountSkippedDueToNoAmount")]
        private partial void LogCountSkippedDueToNoAmount(
             [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordTags), OmitReferenceName = true)]
             ConsumeContext<DisputeApproved> context,
             [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordCountNumberTag), OmitReferenceName = true)]
             int count);
    }
}
