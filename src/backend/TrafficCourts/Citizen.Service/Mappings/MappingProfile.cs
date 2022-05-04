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
                .ForMember(dest => dest.DisputedCounts, opt => opt.MapFrom(src => src.DisputedCounts))
                .ForMember(dest => dest.ViolationTicket, opt => opt.MapFrom(src => src.ViolationTicket))
                .ForPath(dest => dest.ViolationTicket.ViolationTicketCounts, opt => opt.MapFrom(src => src.ViolationTicket.ViolationTicketCounts));
            CreateMap<Models.Dispute.DisputedCount, Messaging.MessageContracts.DisputedCount>();
            CreateMap<Models.Dispute.ViolationTicket, Messaging.MessageContracts.ViolationTicket>();
            CreateMap<Models.Dispute.TicketCount, Messaging.MessageContracts.TicketCount>();
            CreateMap<Models.Dispute.LegalRepresentation, Messaging.MessageContracts.LegalRepresentation>();
        }
    }
}
