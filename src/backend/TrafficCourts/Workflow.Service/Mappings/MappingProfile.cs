using AutoMapper;

namespace TrafficCourts.Workflow.Service.Mappings;

public class MessageContractToDisputeMappingProfile : Profile
{
    public MessageContractToDisputeMappingProfile()
    {
        CreateMap<Messaging.MessageContracts.SubmitNoticeOfDispute, Common.OpenAPIs.OracleDataApi.v1_0.Dispute>();
        CreateMap<Messaging.MessageContracts.DisputedCount, Common.OpenAPIs.OracleDataApi.v1_0.DisputedCount>();
        CreateMap<Messaging.MessageContracts.ViolationTicket, Common.OpenAPIs.OracleDataApi.v1_0.ViolationTicket>()
            // Automapper can't map from DateOnly -> DateTimeOffset
            .ForMember(dest => dest.Birthdate, opt => opt.MapFrom(src => new DateTimeOffset(new DateTime( src.Birthdate.Year, src.Birthdate.Month, src.Birthdate.Day))));
        CreateMap<Messaging.MessageContracts.TicketCount, Common.OpenAPIs.OracleDataApi.v1_0.ViolationTicketCount>();
        CreateMap<Messaging.MessageContracts.LegalRepresentation, Common.OpenAPIs.OracleDataApi.v1_0.LegalRepresentation>();

        CreateMap<DateOnly, DateTime>();
    }
}
