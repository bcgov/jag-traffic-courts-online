using AutoMapper;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Workflow.Service.Models;

namespace TrafficCourts.Workflow.Service.Mappings
{
    public class MessageContractToNoticeOfDisputeMappingProfile : Profile
    {
        public MessageContractToNoticeOfDisputeMappingProfile()
        {
            CreateMap<SubmitNoticeOfDispute, NoticeOfDispute>()
                .ForMember(dest => dest.DisputedCounts, opt => opt.MapFrom(src => src.DisputedCounts))
                .ForMember(dest => dest.ViolationTicket, opt => opt.MapFrom(src => src.ViolationTicket))
                .ForPath(dest => dest.ViolationTicket.ViolationTicketCounts, opt => opt.MapFrom(src => src.ViolationTicket.ViolationTicketCounts))
                // Parsing DateOfBirth to DateTime due to the DateOnly deserialization issues on oracle-data-api
                .ForPath(dest => dest.ViolationTicket.Birthdate, opt => opt.MapFrom(src => src.ViolationTicket.Birthdate));
            CreateMap<Messaging.MessageContracts.DisputedCount, Models.DisputedCount>();
            CreateMap<Messaging.MessageContracts.ViolationTicket, Models.ViolationTicket>();
            CreateMap<Messaging.MessageContracts.TicketCount, Models.ViolationTicketCount>();
            CreateMap<Messaging.MessageContracts.LegalRepresentation, Models.LegalRepresentation>();
            CreateMap<DateOnly, DateTime>();
        }
    }
}
