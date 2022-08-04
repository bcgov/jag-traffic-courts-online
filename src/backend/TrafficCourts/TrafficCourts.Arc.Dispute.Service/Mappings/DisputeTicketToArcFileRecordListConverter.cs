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

            foreach (TicketCount ticket in source.TicketDetails)
            {
                AdnotatedTicket adnotated = new();

                // Adnotated ticket's Master File Data mapping
                adnotated.TransactionDate = source.TicketIssuanceDate;
                adnotated.TransactionTime = source.TicketIssuanceDate;
                adnotated.EffectiveDate = source.TicketIssuanceDate;
                adnotated.FileNumber = source.TicketFileNumber + " 01".ToUpper();
                adnotated.MvbClientNumber = DriversLicence.WithCheckDigit(source.DriversLicence);
                
                // Map adnotated ticket specific data
                adnotated.Name = ReverseName(source.CitizenName);

                if (!string.IsNullOrEmpty(ticket.Section))
                {
                    adnotated.Section = ticket.Section.ToUpper();
                    adnotated.Subsection = ticket.Subsection?.ToUpper() ?? String.Empty;
                    adnotated.Paragraph = ticket.Paragraph?.ToUpper() ?? String.Empty;
                } 
                else
                {
                    LegalSection? legalSection = null;

                    if (ticket.FullSection is not null)
                    {
                        // really doesn't matter if this is true or false, will default
                        // to empty string could not be parsed.
                        LegalSection.TryParse(ticket.FullSection, out legalSection);
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

                    DisputedTicket disputed = new()
                    {
                        // Dispited ticket's Master File Data mapping
                        TransactionDate = source.TicketIssuanceDate,
                        TransactionTime = source.TicketIssuanceDate,
                        EffectiveDate = source.TicketIssuanceDate,
                        FileNumber = source.TicketFileNumber.ToUpper() + " 01",
                        MvbClientNumber = source.DriversLicence.ToUpper(),

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
    }
}
