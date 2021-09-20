using AutoMapper;
using Gov.CitizenApi.Features.Lookups;
using Gov.CitizenApi.Features.Tickets.Commands;
using Gov.CitizenApi.Models;
using Gov.TicketSearch;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Gov.CitizenApi.Features.Tickets.Mapping
{
    public class TicketOffencesResolver : IValueResolver<CreateShellTicketCommand, DBModel.Ticket,
        ICollection<DBModel.Offence>>
    {
        private ILookupsService _lookupsService;
        public TicketOffencesResolver(ILookupsService lookupsService)
        {
            _lookupsService = lookupsService ?? throw new ArgumentNullException(nameof(lookupsService));
        }

        public ICollection<DBModel.Offence> Resolve(CreateShellTicketCommand source,
            DBModel.Ticket destination, ICollection<DBModel.Offence> destMember,
            ResolutionContext context)
        {
            var offences = new Collection<DBModel.Offence>();
            
            if(source.Count1Charge != null && source.Count1FineAmount != null)
            {               
                offences.Add(CreateOffence(1, source.Count1FineAmount.Value, source.Count1Charge.Value, source.ViolationDate, source.ViolationTime));
            }

            if (source.Count2Charge != null && source.Count2FineAmount != null)
            {
                offences.Add(CreateOffence(2, source.Count2FineAmount.Value, source.Count2Charge.Value, source.ViolationDate, source.ViolationTime));
            }
            
            if (source.Count3Charge != null && source.Count3FineAmount != null)
            {
                offences.Add(CreateOffence(3, source.Count3FineAmount.Value, source.Count3Charge.Value, source.ViolationDate, source.ViolationTime));
            }

            return offences.Count > 0 ? offences : null;
        }

        private DBModel.Offence CreateOffence(int offenceNumber, decimal fineAmount, decimal chargeCode, string ticketViolationDate, string ticketViolationTime)
        {
            DateTime datetime;
            DateTime offenceDateTime=DateTime.MinValue;
            bool success = DateTime.TryParse(ticketViolationDate, out datetime);
            if (success) {
                var time = TimeSpan.Parse($"{ticketViolationTime}:00");
                offenceDateTime = datetime.AddHours(time.Hours).AddMinutes(time.Minutes);
            }

            DBModel.Offence offence = new DBModel.Offence
            {
                OffenceNumber = offenceNumber,
                TicketedAmount = fineAmount,
                AmountDue = fineAmount,
                OffenceCode = chargeCode,
                OffenceDescription = _lookupsService.GetCountStatute(chargeCode).Name,
                DiscountAmount = Keys.TicketDiscountValue,
                ViolationDateTime = offenceDateTime == DateTime.MinValue ? null : offenceDateTime.ToString(Keys.DateTimeFormat)
            };
            try
            {
                offence.DiscountDueDate = DateTime.Parse(ticketViolationDate).AddDays(Keys.TicketDiscountValidDays).ToString(Keys.DateTimeFormat);
            }catch(Exception)
            {
                offence.DiscountDueDate = null;
            }
            return offence;
        }
    }

    public class TicketDisputeDiscountAmountResolver : IValueResolver<TicketSearchResponse, TicketDispute, decimal>
    {
        public decimal Resolve(TicketSearchResponse source, TicketDispute destination, decimal destMember, ResolutionContext context)
        {
            if(source.Offences==null || source.Offences.Count==0)
                return 0;
            string discountDueDate = source.Offences.FirstOrDefault(m => !string.IsNullOrWhiteSpace(m.DiscountDueDate))?.DiscountDueDate;
            if ( discountDueDate != null)
            {
                DateTime date;
                bool success = DateTime.TryParse(discountDueDate, out date);
                if(success && DateTime.UtcNow > date) //already pass the discount date
                {
                    return 0;
                }
                decimal discountAmount = 0;
                foreach (Gov.TicketSearch.Offence offence in source.Offences)
                {
                    discountAmount += Convert.ToDecimal(offence.DiscountAmount);
                }
                return discountAmount;
            }
            else
            {
                return 0;
            }
           
        }
    }

    public class TicketDisputeDiscountDueDateResolver : IValueResolver<TicketSearchResponse, TicketDispute, string>
    {
        public string Resolve(TicketSearchResponse source, TicketDispute destination, string destMember, ResolutionContext context)
        {
            if (source.Offences == null || source.Offences.Count == 0)
                return null;
            return source.Offences.FirstOrDefault(m => !string.IsNullOrWhiteSpace(m.DiscountDueDate))?.DiscountDueDate;
        }
    }
}
