using AutoMapper;
using Gov.CitizenApi.Features.Tickets.Commands;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Gov.CitizenApi.Features.Tickets.Mapping
{
    public class TicketOffencesResolver : IValueResolver<CreateShellTicketCommand, DBModel.Ticket,
        ICollection<DBModel.Offence>>
    {
        public ICollection<DBModel.Offence> Resolve(CreateShellTicketCommand source,
            DBModel.Ticket destination, ICollection<DBModel.Offence> destMember,
            ResolutionContext context)
        {
            var offences = new Collection<DBModel.Offence>();
            if(source.Count1Charge != null)
            {
                DBModel.Offence offence = new DBModel.Offence
                {
                    OffenceNumber = 1,
                    TicketedAmount = source.Count1FineAmount,
                    OffenceCode = source.Count1Charge
                };
                offences.Add(offence);
            }
            if (source.Count2Charge != null)
            {
                DBModel.Offence offence = new DBModel.Offence
                {
                    OffenceNumber = 2,
                    TicketedAmount = source.Count2FineAmount,
                    OffenceCode = source.Count2Charge
                };
                offences.Add(offence);
            }
            if (source.Count3Charge != null)
            {
                DBModel.Offence offence = new DBModel.Offence
                {
                    OffenceNumber = 3,
                    TicketedAmount = source.Count3FineAmount,
                    OffenceCode = source.Count3Charge
                };
                offences.Add(offence);
            }
            return offences.Count > 0 ? offences : null;
        }
    }
}