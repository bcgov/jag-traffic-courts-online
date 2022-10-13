using AutoMapper;

namespace TrafficCourts.Workflow.Service.Mappings;

public class MessageContractToDisputeMappingProfile : Profile
{
    public MessageContractToDisputeMappingProfile()
    {
        CreateMap<Messaging.MessageContracts.SubmitNoticeOfDispute, Common.OpenAPIs.OracleDataApi.v1_0.Dispute>();
        CreateMap<Messaging.MessageContracts.DisputeCount, Common.OpenAPIs.OracleDataApi.v1_0.DisputeCount>();
        CreateMap<Messaging.MessageContracts.ViolationTicket, Common.OpenAPIs.OracleDataApi.v1_0.ViolationTicket>()
            // Automapper can't map from DateOnly -> DateTimeOffset
            .ForMember(dest => dest.DisputantBirthdate, opt => opt.MapFrom(src => new DateTimeOffset(new DateTime( src.DisputantBirthdate.Year, src.DisputantBirthdate.Month, src.DisputantBirthdate.Day))));
        CreateMap<Messaging.MessageContracts.TicketCount, Common.OpenAPIs.OracleDataApi.v1_0.ViolationTicketCount>();
        //CreateMap<Messaging.MessageContracts.SendEmail, Common.OpenAPIs.OracleDataApi.v1_0.EmailHistory>();
        CreateMap<Messaging.MessageContracts.SaveFileHistoryRecord, Common.OpenAPIs.OracleDataApi.v1_0.FileHistory> ();

        CreateMap<DateOnly, DateTime>();
    }
}
