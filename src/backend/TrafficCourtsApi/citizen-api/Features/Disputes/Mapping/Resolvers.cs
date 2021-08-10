using AutoMapper;
using Gov.CitizenApi.Features.Disputes.Commands;
using Gov.CitizenApi.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Gov.CitizenApi.Features.Disputes.Mapping
{
    public class OffenceDisputeDetailsResolver : IValueResolver<CreateDisputeCommand, DBModel.Dispute,
        ICollection<DBModel.OffenceDisputeDetail>>
    {
        public ICollection<DBModel.OffenceDisputeDetail> Resolve(CreateDisputeCommand source,
            DBModel.Dispute destination, ICollection<DBModel.OffenceDisputeDetail> destMember,
            ResolutionContext context)
        {
            if (source.Offences == null) return null;
            var offenceDisputeDetails = new Collection<DBModel.OffenceDisputeDetail>();
            foreach (Offence offence in source.Offences)
            {
                var detail = context.Mapper.Map<DBModel.OffenceDisputeDetail>(offence);
                detail.OffenceNumber = offence.OffenceNumber;
                detail.Status = TrafficCourts.Common.Contract.DisputeStatus.Submitted;
                offenceDisputeDetails.Add(detail);
            }

            return offenceDisputeDetails.Count > 0 ? offenceDisputeDetails : null;
        }
    }
}