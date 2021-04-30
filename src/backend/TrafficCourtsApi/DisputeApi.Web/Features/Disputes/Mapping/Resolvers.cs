using AutoMapper;
using DisputeApi.Web.Features.Disputes.Commands;
using DisputeApi.Web.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DisputeApi.Web.Features.Disputes.Mapping
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
                if (offence.OffenceDisputeDetail != null)
                {
                    var detail = context.Mapper.Map<DBModel.OffenceDisputeDetail>(offence.OffenceDisputeDetail);
                    detail.OffenceNumber = offence.OffenceNumber;
                    detail.Status = TrafficCourts.Common.Contract.DisputeStatus.New;
                    offenceDisputeDetails.Add(detail);
                }
            }

            return offenceDisputeDetails.Count > 0 ? offenceDisputeDetails : null;
        }
    }
}