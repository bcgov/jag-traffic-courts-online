using AutoMapper;
using TrafficCourts.Arc.Dispute.Service.Models;
using TrafficCourts.Common;

namespace TrafficCourts.Arc.Dispute.Service.Mappings
{
    public class CustomMap : ITypeConverter<TcoDisputeTicket, List<ArcFileRecord>>
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
                // Mapping adnotated ticket specific data
                adnotated.Name = source.CitizenName;
                if (!string.IsNullOrEmpty(ticket.Section))
                {
                    adnotated.Section = ticket.Section;
                    adnotated.Subsection = ticket.Subsection != null ? ticket.Subsection : "";
                    adnotated.Paragraph = ticket.Paragraph != null ? ticket.Paragraph : "";
                } 
                else
                {
                    if (LegalSection.TryParse(ticket.FullSection, out LegalSection? legalSection))
                    {
                        adnotated.Section = legalSection!.Section;
                        adnotated.Subsection = legalSection.Subsection;
                        adnotated.Paragraph = legalSection.Paragraph;
                    }
                }

                adnotated.Act = ticket.Act != null ? ticket.Act : "MVA";
                adnotated.OriginalAmount = ticket.Amount;
                adnotated.Organization = source.IssuingOrganization;
                adnotated.OrganizationLocation = source.IssuingLocation;
                adnotated.ServiceDate = source.TicketIssuanceDate;

                arcFileRecordList.Add(adnotated);
                // Check if there are data required to encapsulate citizen dispute information
                if (source.DisputeCounts != null && source.DisputeCounts.Count != 0)
                {
                    // precondition is that there is no duplicated counts,
                    // this will throw exception if contained duplicate by count number
                    var disputeCount = source.DisputeCounts.SingleOrDefault(_ => _.Count == ticket.Count);
                    if (disputeCount is null)
                    {
                        continue;
                    }

                    DisputedTicket disputed = new();
                    // Dispited ticket's Master File Data mapping
                    disputed.TransactionDate = source.TicketIssuanceDate;
                    disputed.TransactionTime = source.TicketIssuanceDate;
                    disputed.FileNumber = source.TicketFileNumber;
                    disputed.MvbClientNumber = source.DriversLicence;
                    // Mapping disputed ticket specific data
                    disputed.ServiceDate = source.TicketIssuanceDate;
                    disputed.Name = source.CitizenName;
                    disputed.DisputeType = disputeCount.DisputeType != null ? disputeCount.DisputeType : "A"; //TODO: Find out what dispute type actually means
                    disputed.StreetAddress = source.StreetAddress != null ? source.StreetAddress : "";
                    disputed.City = source.City != null ? source.City : "";
                    disputed.Province = source.Province != null ? source.Province : "";
                    disputed.PostalCode = source.PostalCode != null ? source.PostalCode : "";

                    arcFileRecordList.Add(disputed);
                }              
            }
            return arcFileRecordList;
        }

        ///// <summary>
        ///// Parses out the full section from the TicketCount source and splits into 3 separate section groups required by ARC
        ///// </summary>
        ///// <param name="ticket"></param>
        ///// <param name="fullSection"></param>
        ///// <returns></returns>
        //internal static AdnotatedTicket ParseFullSection (AdnotatedTicket ticket, string fullSection)
        //{
        //    ArgumentNullException.ThrowIfNull(ticket);
        //    ArgumentNullException.ThrowIfNull(fullSection);

        //    var sectionArray = fullSection.Split(new string[] { "(", ")" }, StringSplitOptions.RemoveEmptyEntries);
        //    var count = sectionArray.Length;

        //    if (count > 0)
        //    {
        //        switch (count)
        //        {
        //            case 1:
        //                ticket.Section = sectionArray[0];
        //                break;
        //            case 2:
        //                ticket.Section = sectionArray[0];
        //                ticket.Subsection = sectionArray[1];
        //                break;
        //            case int _ when count > 2:
        //                ticket.Section = sectionArray[0];
        //                ticket.Subsection = sectionArray[1];
        //                ticket.Paragraph = sectionArray[2];
        //                break;
        //            default:
        //                break;
        //        }
        //    }

        //    return ticket;
        //}

    }
}
