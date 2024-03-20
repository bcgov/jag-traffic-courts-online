using AutoMapper;
using TrafficCourts.Domain.Models;

namespace TrafficCourts.Workflow.Service.Mappings;

public class MessageContractToDisputeMappingProfile : Profile
{
    public MessageContractToDisputeMappingProfile()
    {
        CreateMap<Messaging.MessageContracts.SubmitNoticeOfDispute, Domain.Models.Dispute>()
            .ForMember(dest => dest.NoticeOfDisputeGuid, opt => opt.MapFrom(src => src.NoticeOfDisputeGuid.ToString("d")))
            .ForMember(dest => dest.DriversLicenceIssuedCountryId, opt => opt.MapFrom(src => src.DriversLicenceCountryId))
            .ForMember(dest => dest.DriversLicenceIssuedProvinceSeqNo, opt => opt.MapFrom(src => src.DriversLicenceProvinceSeqNo));
        
        CreateMap<Messaging.MessageContracts.DisputeCount, Domain.Models.DisputeCount>();
        
        CreateMap<Messaging.MessageContracts.ViolationTicket, Domain.Models.ViolationTicket>()
            // Automapper can't map from DateOnly -> DateTimeOffset
            .ForMember(dest => dest.DisputantBirthdate, opt => opt.MapFrom(src => new DateTimeOffset(new DateTime(src.DisputantBirthdate.Year, src.DisputantBirthdate.Month, src.DisputantBirthdate.Day))));
        
        CreateMap<Messaging.MessageContracts.TicketCount, Domain.Models.ViolationTicketCount>();
        
        CreateMap<Messaging.MessageContracts.SaveFileHistoryRecord, Domain.Models.FileHistory>();

        CreateMap<Domain.Models.Dispute, Messaging.MessageContracts.SubmitNoticeOfDispute>()
            .ForMember(dest => dest.NoticeOfDisputeGuid, opt => opt.MapFrom(src => Guid.Parse(src.NoticeOfDisputeGuid)))
            .ForMember(dest => dest.DriversLicenceCountryId, opt => opt.MapFrom(src => src.DriversLicenceIssuedCountryId))
            .ForMember(dest => dest.DriversLicenceProvinceSeqNo, opt => opt.MapFrom(src => src.DriversLicenceIssuedProvinceSeqNo));

        CreateMap<Domain.Models.DisputeCount, Messaging.MessageContracts.DisputeCount>();

        CreateMap<Domain.Models.ViolationTicket, Messaging.MessageContracts.ViolationTicket>()
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
