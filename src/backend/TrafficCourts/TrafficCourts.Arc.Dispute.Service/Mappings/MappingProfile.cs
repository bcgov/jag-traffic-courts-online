using AutoMapper;
using TrafficCourts.Arc.Dispute.Service.Models;

namespace TrafficCourts.Arc.Dispute.Service.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TcoDisputeTicket, List<ArcFileRecord>>().ConvertUsing<DisputeTicketToArcFileRecordListConverter>();
        }
    }
}
