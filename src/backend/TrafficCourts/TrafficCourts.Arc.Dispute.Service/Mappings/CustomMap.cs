using AutoMapper;
using TrafficCourts.Arc.Dispute.Service.Models;

namespace TrafficCourts.Arc.Dispute.Service.Mappings
{
    public class CustomMap : ITypeConverter<TcoDisputeTicket, List<ArcFileRecord>>
    {
        public List<ArcFileRecord> Convert(TcoDisputeTicket source, List<ArcFileRecord> destination, ResolutionContext context)
        {
            List<ArcFileRecord> arcFileRecordList = new List<ArcFileRecord>();

            foreach (TicketDetails ticket in source.TicketDetails)
            {
                AdnotatedTicket adnotated = new AdnotatedTicket();
                // Adnotated ticket's Master File Data mapping
                adnotated.TransactionDate = source.TicketIssuanceDate;
                adnotated.TransactionTime = source.TicketIssuanceDate;
                adnotated.FileNumber = source.TicketFileNumber;
                adnotated.MvbClientNumber = source.DriversLicense;
                adnotated.Organization = source.IssuingOrganization;
                adnotated.OrganizationLocation = source.IssuingLocation;
                adnotated.Name = source.CitizenName;
                // Mapping adnotated ticket specific data
                adnotated.Section = ticket.section;
                adnotated.Subsection = ticket.subsection;
                adnotated.Paragraph = ticket.paragraph;
                adnotated.Act = ticket.act;
                adnotated.OriginalAmount = ticket.amount;
                // Create a dispute key to check if the corresponding dispute data is in the dictionary
                var disputeKey = CreateDisputeKey(ticket.section, ticket.subsection, ticket.paragraph, ticket.act);

                arcFileRecordList.Add(adnotated);
                // Check if there are data required to encapsulate citizen dispute information
                if (source.DisputeDetails != null && source.DisputeDetails.Length > 0)
                {
                    foreach (var disputeDet in source.DisputeDetails)
                    {
                        // If citizen dispute data has been found, then create arc file disputed row for each disputed ticket count
                        if (disputeDet.TryGetValue(disputeKey, out var disputeValue))
                        {
                            DisputedTicket disputed = new DisputedTicket();
                            // Dispited ticket's Master File Data mapping
                            disputed.TransactionDate = source.TicketIssuanceDate;
                            disputed.TransactionTime = source.TicketIssuanceDate;
                            disputed.FileNumber = source.TicketFileNumber;
                            disputed.MvbClientNumber = source.DriversLicense;
                            disputed.Name = source.CitizenName;
                            // Mapping disputed ticket specific data
                            disputed.DisputeType = disputeValue.DisputeType;
                            disputed.StreetAddress = source.StreetAddress;
                            disputed.City = source.City;
                            disputed.Province = source.Province;
                            disputed.PostalCode = source.PostalCode;

                            arcFileRecordList.Add(disputed);

                        }
                    }
                }              
            }
            return arcFileRecordList;
        }

        public string CreateDisputeKey (string section, string subsection, string paragraph, string act)
        {
            var disputeKey = $"{section}.{subsection}.{paragraph}.{act}";
            
            return disputeKey;
        }
    }
}