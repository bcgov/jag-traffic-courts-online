using AutoMapper;
using TrafficCourts.Citizen.Service.Models.Disputes;
using TrafficCourts.Messaging.MessageContracts;

namespace TrafficCourts.Citizen.Service.Mappings;

public class NoticeOfDisputeToMessageContractMappingProfile : Profile
{
    public NoticeOfDisputeToMessageContractMappingProfile()
    {
        CreateMap<NoticeOfDispute, SubmitNoticeOfDispute>()
            .ForMember(dest => dest.DisputeCounts, opt => opt.MapFrom(src => src.DisputeCounts));
        CreateMap<Models.Disputes.DisputeCount, Messaging.MessageContracts.DisputeCount>();

        CreateMap<SubmitNoticeOfDispute, NoticeOfDispute> ()
            .ForMember(dest => dest.DisputeCounts, opt => opt.MapFrom(src => src.DisputeCounts));
        CreateMap<Messaging.MessageContracts.DisputeCount, Models.Disputes.DisputeCount>();

        CreateMap<Models.Tickets.ViolationTicket, Messaging.MessageContracts.ViolationTicket>()
            .ForMember(dest => dest.ViolationTicketCounts, opt => opt.MapFrom(src => src.Counts));
        CreateMap<Models.Tickets.ViolationTicketCount, Messaging.MessageContracts.TicketCount>()
            .ForMember(dest => dest.Section, opt => opt.MapFrom(src => src.Section));

        CreateMap<DisputantContactInformation, Messaging.MessageContracts.DisputantUpdateContactRequest>();
        CreateMap<Messaging.MessageContracts.DisputantUpdateContactRequest, Messaging.MessageContracts.DisputantUpdateRequest>();

        CreateMap<Dispute, Messaging.MessageContracts.DisputantUpdateRequest>()
            .ForMember(dest => dest.RepresentedByLawyer, opt => opt.MapFrom(src => src.RepresentedByLawyer == Common.OpenAPIs.OracleDataApi.v1_0.DisputeRepresentedByLawyer.Y ? true : false))
            .ForMember(dest => dest.InterpreterRequired, opt => opt.MapFrom(src => src.InterpreterRequired == Common.OpenAPIs.OracleDataApi.v1_0.DisputeInterpreterRequired.Y ? true : false))
            .ForMember(dest => dest.DisputeCounts, opt => opt.MapFrom(src => src.DisputeCounts));
        CreateMap<Models.Disputes.DisputeCount, Common.OpenAPIs.OracleDataApi.v1_0.DisputeCount>();
    }
}
