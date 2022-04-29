using AutoMapper;
using TrafficCourts.Arc.Dispute.Service.Models;

namespace TrafficCourts.Arc.Dispute.Service.Mappings
{
    public class CustomMap : ITypeConverter<TcoDisputeTicket, List<ArcFileRecord>>
    {
        public List<ArcFileRecord> Convert(TcoDisputeTicket source, List<ArcFileRecord> destination, ResolutionContext context)
        {
            ArgumentNullException.ThrowIfNull(source);

            List<ArcFileRecord> arcFileRecordList = new List<ArcFileRecord>();

            foreach (TicketCount ticket in source.TicketDetails)
            {
                AdnotatedTicket adnotated = new AdnotatedTicket();
                // Adnotated ticket's Master File Data mapping
                adnotated.TransactionDate = source.TicketIssuanceDate;
                adnotated.TransactionTime = source.TicketIssuanceDate;
                adnotated.FileNumber = source.TicketFileNumber;
                adnotated.MvbClientNumber = source.DriversLicence;
                // Mapping adnotated ticket specific data
                adnotated.Name = source.CitizenName;
                adnotated.Section = ticket.Section;
                adnotated.Subsection = ticket.Subsection;
                adnotated.Paragraph = ticket.Paragraph;
                adnotated.Act = ticket.Act;
                adnotated.OriginalAmount = ticket.Amount;
                adnotated.Organization = source.IssuingOrganization;
                adnotated.OrganizationLocation = source.IssuingLocation;

                arcFileRecordList.Add(adnotated);
                // Check if there are data required to encapsulate citizen dispute information
                if (source.DisputeCounts != null && source.DisputeCounts.Any())
                {
                    foreach (var disputeCount in source.DisputeCounts)
                    {
                        // If citizen dispute data count matches violation ticket count, then create arc file disputed row for each disputed ticket count
                        if (ticket.Count == disputeCount.Count)
                        {
                            DisputedTicket disputed = new DisputedTicket();
                            // Dispited ticket's Master File Data mapping
                            disputed.TransactionDate = source.TicketIssuanceDate;
                            disputed.TransactionTime = source.TicketIssuanceDate;
                            disputed.FileNumber = source.TicketFileNumber;
                            disputed.MvbClientNumber = source.DriversLicence;
                            // Mapping disputed ticket specific data
                            disputed.Name = source.CitizenName;
                            disputed.DisputeType = disputeCount.DisputeType != null ? disputeCount.DisputeType : "A"; //TODO: Find out what dispute type actually means
                            disputed.StreetAddress = source.StreetAddress != null ? source.StreetAddress : "";
                            disputed.City = source.City != null ? source.City : "";
                            disputed.Province = source.Province != null ? source.Province : "";
                            disputed.PostalCode = source.PostalCode != null ? source.PostalCode : "";

                            arcFileRecordList.Add(disputed);
                        }
                    }
                }              
            }
            return arcFileRecordList;
        }

    }
}
