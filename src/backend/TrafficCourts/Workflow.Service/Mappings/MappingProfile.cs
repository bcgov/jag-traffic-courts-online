using AutoMapper;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;

namespace TrafficCourts.Workflow.Service.Mappings;

public class MessageContractToDisputeMappingProfile : Profile
{
    public MessageContractToDisputeMappingProfile()
    {
        CreateMap<Messaging.MessageContracts.SubmitNoticeOfDispute, Common.OpenAPIs.OracleDataApi.v1_0.Dispute>()
            .ForMember(dest => dest.NoticeOfDisputeGuid, opt => opt.MapFrom(src => src.NoticeOfDisputeGuid.ToString("d")))
            .ForMember(dest => dest.DriversLicenceIssuedCountryId, opt => opt.MapFrom(src => src.DriversLicenceCountryId))
            .ForMember(dest => dest.DriversLicenceIssuedProvinceSeqNo, opt => opt.MapFrom(src => src.DriversLicenceProvinceSeqNo));
        
        CreateMap<Messaging.MessageContracts.DisputeCount, Common.OpenAPIs.OracleDataApi.v1_0.DisputeCount>();
        
        CreateMap<Messaging.MessageContracts.ViolationTicket, Common.OpenAPIs.OracleDataApi.v1_0.ViolationTicket>()
            // Automapper can't map from DateOnly -> DateTimeOffset
            .ForMember(dest => dest.DisputantBirthdate, opt => opt.MapFrom(src => new DateTimeOffset(new DateTime(src.DisputantBirthdate.Year, src.DisputantBirthdate.Month, src.DisputantBirthdate.Day))));
        
        CreateMap<Messaging.MessageContracts.TicketCount, Common.OpenAPIs.OracleDataApi.v1_0.ViolationTicketCount>();
        
        CreateMap<Messaging.MessageContracts.SaveFileHistoryRecord, Common.OpenAPIs.OracleDataApi.v1_0.FileHistory>();

        CreateMap<Common.OpenAPIs.OracleDataApi.v1_0.Dispute, Messaging.MessageContracts.SubmitNoticeOfDispute>()
            .ForMember(dest => dest.NoticeOfDisputeGuid, opt => opt.MapFrom(src => Guid.Parse(src.NoticeOfDisputeGuid)))
            .ForMember(dest => dest.DriversLicenceCountryId, opt => opt.MapFrom(src => src.DriversLicenceIssuedCountryId))
            .ForMember(dest => dest.DriversLicenceProvinceSeqNo, opt => opt.MapFrom(src => src.DriversLicenceIssuedProvinceSeqNo));

        CreateMap<Common.OpenAPIs.OracleDataApi.v1_0.DisputeCount, Messaging.MessageContracts.DisputeCount>();

        CreateMap<Common.OpenAPIs.OracleDataApi.v1_0.ViolationTicket, Messaging.MessageContracts.ViolationTicket>()
            .ForMember(dest => dest.ViolationTicketCounts, opt => opt.MapFrom(src => src.ViolationTicketCounts));

        CreateMap<DateOnly, DateTime>();

        CreateMap<Dispute, Messaging.MessageContracts.DisputeUpdateRequest>()
            .ForMember(dest => dest.RepresentedByLawyer, opt => opt.MapFrom(src => src.RepresentedByLawyer == DisputeRepresentedByLawyer.Y ? true : false))
            .ForMember(dest => dest.InterpreterRequired, opt => opt.MapFrom(src => src.InterpreterRequired == DisputeInterpreterRequired.Y ? true : false))
            .ForMember(dest => dest.DriversLicenceNumber, opt => opt.MapFrom(src => src.DriversLicenceNumber))
            .ForMember(dest => dest.DriversLicenceIssuedCountryId, opt => opt.MapFrom(src => src.DriversLicenceIssuedCountryId))
            .ForMember(dest => dest.DriversLicenceIssuedProvinceSeqNo, opt => opt.MapFrom(src => src.DriversLicenceIssuedProvinceSeqNo))
            .ForMember(dest => dest.DriversLicenceProvince, opt => opt.MapFrom(src => src.DriversLicenceProvince))
            .ForMember(dest => dest.RequestCourtAppearance, opt => opt.MapFrom(src => src.RequestCourtAppearanceYn))
            .ForMember(dest => dest.DisputeCounts, opt => opt.MapFrom(src => src.DisputeCounts))
            .ForMember(dest => dest.ContactType, opt => opt.MapFrom(src => src.ContactTypeCd));
    }
}
