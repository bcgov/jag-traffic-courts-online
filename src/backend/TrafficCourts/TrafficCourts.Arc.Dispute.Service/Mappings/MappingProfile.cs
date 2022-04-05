using AutoMapper;
using TrafficCourts.Arc.Dispute.Service.Models;

namespace TrafficCourts.Arc.Dispute.Service.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            /*
            CreateMap<TcoDisputeTicket, ArcFileRecord>()
                .ForMember(dest => dest.TransactionDate, opt => opt.MapFrom(src => src.TicketIssuanceDate))
                .ForMember(dest => dest.FileNumber, opt => opt.MapFrom(src => src.TicketFileNumber))
                .ForMember(dest => dest.MvbClientNumber, opt => opt.MapFrom(src => src.DriversLicence))
                .IncludeAllDerived();

            CreateMap<TcoDisputeTicket, AdnotatedTicket>()
                .ForMember(dest => dest.Organization, opt => opt.MapFrom(src => src.IssuingOrganization))
                .ForMember(dest => dest.OrganizationLocation, opt => opt.MapFrom(src => src.IssuingLocation));

            CreateMap<TcoDisputeTicket, DisputedTicket>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.CitizenName));
            */

            CreateMap<TcoDisputeTicket, List<ArcFileRecord>>().ConvertUsing<CustomMap>();
        }
    }
}
