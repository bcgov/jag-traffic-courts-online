using Swashbuckle.AspNetCore.Annotations;
using System.Diagnostics.CodeAnalysis;

namespace TrafficCourts.Citizen.Service.Models.Search
{
    /// <summary>
    /// Obsolete: Represents a violation ticket that is returned from search requests
    /// </summary>
    [Obsolete]
    public class TicketSearchResult
    {
        public TicketSearchResult(Tickets.ViolationTicket violationTicket)
        {
            if (violationTicket is null)
            {
                throw new ArgumentNullException(nameof(violationTicket));
            }

            if (string.IsNullOrWhiteSpace(violationTicket.TicketNumber))
            {
                throw new ArgumentException("Property TicketNumber cannot be empty", nameof(violationTicket));
            }

            if (violationTicket.IssuedDate is null)
            {
                throw new ArgumentException("Property IssuedDate cannot be null", nameof(violationTicket));
            }

            var date = DateOnly.FromDateTime(violationTicket.IssuedDate.Value);
            var time = TimeOnly.FromDateTime(violationTicket.IssuedDate.Value);

            ViolationTicketNumber = violationTicket.TicketNumber!;
            ViolationDate = new DateTime(date.Year, date.Month, date.Day);
            ViolationTime = $"{time.Hour:d2}:{time.Minute:d2}";

            Offences = violationTicket
                .Counts
                .OrderBy(_ => _.Count)
                .Where(_ => _.TicketedAmount is not null)
                .Select(_ => new TicketOffence
                {
                    OffenceNumber = _.Count,
                    TicketedAmount = _.TicketedAmount!.Value,
                    AmountDue = _.AmountDue ?? _.TicketedAmount.Value,
                    OffenceDescription = _.Description,
                    VehicleDescription = string.Empty,
                    InvoiceType = "Traffic Violation Ticket"
                })
                .ToList();
        }

        /// <summary>
        /// The violation ticket number. This will match the ticket number searched for.
        /// </summary>
        public string ViolationTicketNumber { get; set; }
        /// <summary>
        /// The date the violation ticket was issued.
        /// </summary>
        [SwaggerSchema(Format = "date")]
        public DateTime ViolationDate { get; set; }
        /// <summary>
        /// The time of day the violation ticket was issued. This will match the time searched for.
        /// </summary>
        public string ViolationTime { get; set; }
        /// <summary>
        /// The list of offences on this violation ticket.
        /// </summary>
        public List<TicketOffence> Offences { get; set; }
    }

    public class TicketOffence
    {
        /// <summary>
        /// The offence or count number.
        /// </summary>
        public int OffenceNumber { get; set; }

        /// <summary>
        /// The ticketed amount for this offence.
        /// </summary>
        public decimal TicketedAmount { get; set; }
        /// <summary>
        /// The current amount due.
        /// </summary>
        public decimal AmountDue { get; set; }
        /// <summary>
        /// </summary>
        public string? InvoiceType { get; set; }
        /// <summary>
        /// The description of the offence.
        /// </summary>
        public string? OffenceDescription { get; set; }

        /// <summary>
        /// The description of vehicle.
        /// </summary>
        public string? VehicleDescription { get; set; }
    }

    #region Obsolete



    #endregion

}
