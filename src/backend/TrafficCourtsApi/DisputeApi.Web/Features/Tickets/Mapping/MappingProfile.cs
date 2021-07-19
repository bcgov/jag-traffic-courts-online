using AutoMapper;
using DisputeApi.Web.Models;
using Gov.TicketSearch;
using DisputeOffence = DisputeApi.Web.Models.Offence;
using TicketSearchOffence = Gov.TicketSearch.Offence;

namespace DisputeApi.Web.Features.Tickets.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TicketSearchResponse, TicketDispute>();

            CreateMap<TicketSearchOffence, DisputeOffence>();
        }
    }
}
