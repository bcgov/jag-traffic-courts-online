using AutoMapper;
using System.Text;
using TrafficCourts.Arc.Dispute.Service.Models;
using TrafficCourts.Common;

namespace TrafficCourts.Arc.Dispute.Service.Mappings
{
    /// <summary>
    /// Converts from a <see cref="TcoDisputeTicket"/> to a list of <see cref="ArcFileRecord"/>/
    /// </summary>
    public class DisputeTicketToArcFileRecordListConverter : ITypeConverter<TcoDisputeTicket, List<ArcFileRecord>>
    {
        public List<ArcFileRecord> Convert(TcoDisputeTicket source, List<ArcFileRecord> destination, ResolutionContext context)
        {
            ArgumentNullException.ThrowIfNull(source);

            // Destination is not null only when you pass the destination in the Map call.
            if (destination is null)
            {
                destination = new List<ArcFileRecord>();
            }

            // Additional minutes between the transaction times since each transaction timestamp for EV and ED must be unique for ARC to process
            var now = GetCurrentDate();

            // process each of the ticket counts in "count" order
            foreach (TicketCount ticket in source.TicketDetails.OrderBy(_ => _.Count))
            {
                AdnotatedTicket adnotated = new();

                // Adnotated ticket's Master File Data mapping
                adnotated.TransactionDateTime = now;
                adnotated.EffectiveDate = source.TicketIssuanceDate;
                // There has to be two spaces between the 10th character and the "01" at the end for ARC to process the file properly
                adnotated.FileNumber = source.TicketFileNumber.ToUpper() + "  01";
                adnotated.CountNumber = ticket.Count.ToString("D3"); // left pad with zeros
                adnotated.MvbClientNumber = DriversLicence.WithCheckDigit(source.DriversLicence);
                
                // Map adnotated ticket specific data
                adnotated.Name = GetName(source);

                if (!string.IsNullOrEmpty(ticket.Section))
                {
                    adnotated.Section = ticket.Section.ToUpper();
                    adnotated.Subsection = ticket.Subsection?.ToUpper() ?? String.Empty;
                    adnotated.Paragraph = ticket.Paragraph?.ToUpper() ?? String.Empty;
                } 
                else
                {
                    LegalSection? legalSection = null;

                    if (ticket.Section is not null)
                    {
                        // really doesn't matter if this is true or false, will default
                        // to empty string could not be parsed.
                        LegalSection.TryParse(ticket.Section, out legalSection);
                    }

                    adnotated.Section = legalSection?.Section.ToUpper() ?? String.Empty;
                    adnotated.Subsection = legalSection?.Subsection.ToUpper() ?? String.Empty;
                    adnotated.Paragraph = legalSection?.Paragraph.ToUpper() ?? String.Empty;
                }

                adnotated.Act = ticket.Act?.ToUpper() ?? "MVA";
                adnotated.OriginalAmount = ticket.Amount;
                adnotated.Organization = source.IssuingOrganization.ToUpper();
                adnotated.OrganizationLocation = source.IssuingLocation.ToUpper();
                adnotated.ServiceDate = source.TicketIssuanceDate;

                destination.Add(adnotated);

                // Check if there are data required to encapsulate citizen dispute information
                if (source.DisputeCounts is not null && source.DisputeCounts.Count != 0)
                {
                    // precondition is that there is no duplicated counts,
                    // this will throw exception if contained duplicate by count number
                    var disputeCount = source.DisputeCounts.SingleOrDefault(_ => _.Count == ticket.Count);
                    if (disputeCount is null)
                    {
                        continue;
                    }

                    now = Increment(now);

                    DisputedTicket disputed = new()
                    {
                        // Dispited ticket's Master File Data mapping
                        TransactionDateTime = now,
                        EffectiveDate = source.TicketIssuanceDate,
                        // There has to be two spaces between the 10th character and the "01" at the end for ARC to process the file properly
                        FileNumber = source.TicketFileNumber.ToUpper() + "  01",
                        CountNumber = disputeCount.Count.ToString("D3"), // left pad with zeros
                        MvbClientNumber = DriversLicence.WithCheckDigit(source.DriversLicence),

                        // Mapping disputed ticket specific data
                        ServiceDate = source.TicketIssuanceDate,
                        Name = GetName(source),
                        DisputeType = disputeCount.DisputeType?.Trim().ToUpper() ?? "A", //TODO: Find out what dispute type actually means
                        StreetAddress = source.StreetAddress?.Trim().ToUpper() ?? String.Empty,
                        City = source.City?.Trim().ToUpper() ?? String.Empty,
                        Province = source.Province?.Trim().ToUpper() ?? String.Empty,
                        PostalCode = source.PostalCode?.Trim().ToUpper() ?? String.Empty
                    };

                    destination.Add(disputed);
                }

                now = Increment(now);
            }

            return destination;
        }

        internal static string GetName(TcoDisputeTicket ticket)
        {
            StringBuilder name = new StringBuilder();
            if (AppendIfNotIsNullOrWhiteSpace(name, ticket.Surname))
            {
                name.Append(','); // add comma after surname
            }

            AppendIfNotIsNullOrWhiteSpace(name, ticket.GivenName1);
            AppendIfNotIsNullOrWhiteSpace(name, ticket.GivenName2);
            AppendIfNotIsNullOrWhiteSpace(name, ticket.GivenName3);

            return name.ToString().ToUpper(); // name always needs to be converted to upper case
        }

        private static bool AppendIfNotIsNullOrWhiteSpace(StringBuilder buffer, string? value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                if (buffer.Length != 0)
                {
                    // add a space before the previous text
                    buffer.Append(' ');
                }

                buffer.Append(value.Trim());
                return true;
            }

            return false;
        }

        /// <summary>
        /// Function to get curent date and time, set in unit tests
        /// </summary>
        internal static Func<DateTime> Now = () => DateTime.Now;

        private static DateTime GetCurrentDate()
        {
            DateTime now = Now();
            now = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, DateTimeKind.Local);
            return now;
        }

        /// <summary>
        /// Incements the date so each record has a unique date and time.
        /// </summary>
        private static DateTime Increment(DateTime value) => value.AddSeconds(1);
    }
}
