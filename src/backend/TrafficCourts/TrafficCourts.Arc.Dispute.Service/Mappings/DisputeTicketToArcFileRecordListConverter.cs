using AutoMapper;
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

            List<ArcFileRecord> arcFileRecordList = new();

            // Additional minutes between the transaction times since each transaction timestamp for EV and ED must be unique for ARC to process
            var now = GetCurrentDate();

            foreach (TicketCount ticket in source.TicketDetails)
            {
                AdnotatedTicket adnotated = new();

                // Adnotated ticket's Master File Data mapping
                adnotated.TransactionDate = now;
                adnotated.TransactionTime = now;
                adnotated.EffectiveDate = source.TicketIssuanceDate;
                // There has to be two spaces between the 10th character and the "01" at the end for ARC to process the file properly
                adnotated.FileNumber = source.TicketFileNumber.ToUpper() + "  01";
                adnotated.CountNumber = ticket.Count.ToString("D3"); // left pad with zeros
                adnotated.MvbClientNumber = DriversLicence.WithCheckDigit(source.DriversLicence);
                
                // Map adnotated ticket specific data
                adnotated.Name = ReverseName(source.CitizenName);

                if (!string.IsNullOrEmpty(ticket.Section))
                {
                    adnotated.Section = ticket.Section.ToUpper();
                    adnotated.Subsection = ticket.Subsection?.ToUpper() ?? String.Empty;
                    adnotated.Paragraph = ticket.Paragraph?.ToUpper() ?? String.Empty;
                    adnotated.Subparagraph = ticket.Subparagraph?.ToUpper() ?? String.Empty;
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
                    adnotated.Subparagraph = legalSection?.Subparagrah.ToUpper() ?? String.Empty;
                }

                adnotated.Act = ticket.Act?.ToUpper() ?? "MVA";
                adnotated.OriginalAmount = ticket.Amount;
                adnotated.Organization = source.IssuingOrganization.ToUpper();
                adnotated.OrganizationLocation = source.IssuingLocation.ToUpper();
                adnotated.ServiceDate = source.TicketIssuanceDate;

                arcFileRecordList.Add(adnotated);

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

                    now = now.AddMinutes(1);

                    DisputedTicket disputed = new()
                    {
                        // Dispited ticket's Master File Data mapping
                        TransactionDate = now,
                        TransactionTime = now,
                        EffectiveDate = source.TicketIssuanceDate,
                        // There has to be two spaces between the 10th character and the "01" at the end for ARC to process the file properly
                        FileNumber = source.TicketFileNumber.ToUpper() + "  01",
                        MvbClientNumber = DriversLicence.WithCheckDigit(source.DriversLicence),

                        // Mapping disputed ticket specific data
                        ServiceDate = source.TicketIssuanceDate,
                        Name = ReverseName(source.CitizenName),
                        DisputeType = disputeCount.DisputeType?.ToUpper() ?? "A", //TODO: Find out what dispute type actually means
                        StreetAddress = source.StreetAddress?.ToUpper() ?? String.Empty,
                        City = source.City?.ToUpper() ?? String.Empty,
                        Province = source.Province?.ToUpper() ?? String.Empty,
                        PostalCode = source.PostalCode?.ToUpper() ?? String.Empty
                    };

                    arcFileRecordList.Add(disputed);
                }

                now = now.AddMinutes(1);
            }
            return arcFileRecordList;
        }

        internal static string ReverseName(string name)
        {
            var splitted = name.Split(" ");
            string surname = splitted.Last().ToString().ToUpper() + ", ";
            splitted = splitted.SkipLast(1).ToArray();
            string givenNames = string.Join(" ", splitted).ToString().ToUpper();

            return surname + givenNames;
        }

        private static DateTime GetCurrentDate()
        {
            DateTime date = DateTime.Now;

            // Calculate the number of ticks (1 tick = 100 nanoseconds)
            // that represent 1 second, and add that number of ticks
            // to the current DateTime object to round it down to the
            // nearest whole second.

            // kind of ugly but it works
            long ticksPerSecond = TimeSpan.TicksPerSecond;
            DateTime roundedDate = date.AddTicks(ticksPerSecond / 2 * -1);
            return roundedDate;
        }
    }
}
