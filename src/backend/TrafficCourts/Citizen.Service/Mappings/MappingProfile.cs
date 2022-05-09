using AutoMapper;
using TrafficCourts.Citizen.Service.Models.Dispute;
using TrafficCourts.Messaging.MessageContracts;

namespace TrafficCourts.Citizen.Service.Mappings
{
    public class NoticeOfDisputeToMessageContractMappingProfile : Profile
    {
        public NoticeOfDisputeToMessageContractMappingProfile()
        {
            CreateMap<NoticeOfDispute, SubmitNoticeOfDispute>()
                .ForMember(dest => dest.DisputedCounts, opt => opt.MapFrom(src => src.DisputedCounts));
            CreateMap<Models.Dispute.DisputedCount, Messaging.MessageContracts.DisputedCount>();
            CreateMap<Models.Tickets.ViolationTicket, Messaging.MessageContracts.ViolationTicket>()
                .ForMember(dest => dest.ViolationTicketCounts, opt => opt.MapFrom(src => src.Counts));
            CreateMap<Models.Tickets.ViolationTicketCount, Messaging.MessageContracts.TicketCount>()
                .ForMember(dest => dest.FullSection, opt => opt.MapFrom(src => src.Section));
            CreateMap<Models.Dispute.LegalRepresentation, Messaging.MessageContracts.LegalRepresentation>();
        }
    }
}
