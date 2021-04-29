using AutoMapper;
using DisputeApi.Web.Features.Disputes.Commands;
using DisputeApi.Web.Features.Disputes.DBModel;
using DisputeApi.Web.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace DisputeApi.Web.Features.Disputes.Mapping
{
    public class OffenceDisputeDetailsResolver : IValueResolver<CreateDisputeCommand, DBModel.Dispute, ICollection<DBModel.OffenceDisputeDetail>>
    {
        public ICollection<DBModel.OffenceDisputeDetail> Resolve(CreateDisputeCommand source, DBModel.Dispute destination, ICollection<DBModel.OffenceDisputeDetail> destMember, ResolutionContext context)
        {
            if (source.Offences == null) return null;
            Collection<DBModel.OffenceDisputeDetail> offenceDisputeDetails = new Collection<DBModel.OffenceDisputeDetail>();
            foreach(Offence offence in source.Offences)
            {
                if(offence.OffenceDisputeDetail != null)
                {
                    var detail = context.Mapper.Map<DBModel.OffenceDisputeDetail>(offence.OffenceDisputeDetail);
                    detail.OffenceNumber = offence.OffenceNumber;
                    detail.Status = TrafficCourts.Common.Contract.DisputeStatus.New;
                    offenceDisputeDetails.Add(detail);
                }
            }
            if (offenceDisputeDetails.Count > 0)
            {
                return offenceDisputeDetails;
            }
            return null;
        }
    }
}
