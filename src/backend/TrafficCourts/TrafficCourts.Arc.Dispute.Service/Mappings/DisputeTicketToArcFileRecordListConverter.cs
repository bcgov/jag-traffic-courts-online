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
                adnotated.FileNumber = source.TicketFileNumber;
                adnotated.MvbClientNumber = source.DriversLicence;
                
                // Map adnotated ticket specific data
                adnotated.Name = source.CitizenName;

                if (!string.IsNullOrEmpty(ticket.Section))
                {
                    adnotated.Section = ticket.Section;
                    adnotated.Subsection = ticket.Subsection ?? String.Empty;
                    adnotated.Paragraph = ticket.Paragraph ?? String.Empty;
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

                    adnotated.Section = legalSection?.Section ?? String.Empty;
                    adnotated.Subsection = legalSection?.Subsection ?? String.Empty;
                    adnotated.Paragraph = legalSection?.Paragraph ?? String.Empty;
                }

                adnotated.Act = ticket.Act ?? "MVA";
                adnotated.OriginalAmount = ticket.Amount;
                adnotated.Organization = source.IssuingOrganization;
                adnotated.OrganizationLocation = source.IssuingLocation;
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
                        FileNumber = source.TicketFileNumber,
                        MvbClientNumber = source.DriversLicence,

                        // Mapping disputed ticket specific data
                        ServiceDate = source.TicketIssuanceDate,
                        Name = source.CitizenName,
                        DisputeType = disputeCount.DisputeType ?? "A", //TODO: Find out what dispute type actually means
                        StreetAddress = source.StreetAddress ?? String.Empty,
                        City = source.City ?? String.Empty,
                        Province = source.Province ?? String.Empty,
                        PostalCode = source.PostalCode ?? String.Empty
                    };

                    arcFileRecordList.Add(disputed);
                }              
            }
            return arcFileRecordList;
        }
    }
}
