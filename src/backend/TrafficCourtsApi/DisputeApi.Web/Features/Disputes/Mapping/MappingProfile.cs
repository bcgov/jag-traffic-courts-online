using AutoMapper;
using DisputeApi.Web.Features.Disputes.Commands;
using DisputeApi.Web.Features.Disputes.Queries;
using TrafficCourts.Common.Contract;

namespace DisputeApi.Web.Features.Disputes.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateDisputeCommand, DBModel.Dispute>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.DisputantBirthDate, opt => opt.MapFrom(src => src.Disputant==null? null : src.Disputant.BirthDate))
                .ForMember(dest => dest.DisputantEmailAddress, opt => opt.MapFrom(src => src.Disputant == null ? null : src.Disputant.EmailAddress))
                .ForMember(dest => dest.DisputantFirstName, opt => opt.MapFrom(src => src.Disputant == null ? null : src.Disputant.GivenNames))
                .ForMember(dest => dest.DisputantLastName, opt => opt.MapFrom(src => src.Disputant == null ? null : src.Disputant.LastName))
                .ForMember(dest => dest.DisputantMailingAddress, opt => opt.MapFrom(src => src.Disputant == null ? null : src.Disputant.MailingAddress))
                .ForMember(dest => dest.DisputantMailingCity, opt => opt.MapFrom(src => src.Disputant == null ? null : src.Disputant.City))
                .ForMember(dest => dest.DisputantMailingPostalCode, opt => opt.MapFrom(src => src.Disputant == null ? null : src.Disputant.PostalCode))
                .ForMember(dest => dest.DisputantMailingProvince, opt => opt.MapFrom(src => src.Disputant == null ? null : src.Disputant.Province))
                .ForMember(dest => dest.DriverLicense, opt => opt.MapFrom(src => src.Disputant == null ? null : src.Disputant.DriverLicense))
                .ForMember(dest => dest.DriverLicenseProvince, opt => opt.MapFrom(src => src.Disputant == null ? null : src.Disputant.DriverLicenseProvince))
                .ForMember(dest => dest.HomePhoneNumber, opt => opt.MapFrom(src => src.Disputant == null ? null : src.Disputant.HomePhoneNumber))
                .ForMember(dest => dest.WorkPhoneNumber, opt => opt.MapFrom(src => src.Disputant == null ? null : src.Disputant.WorkPhoneNumber))
                .ForMember(dest => dest.InformationCertified, opt => opt.MapFrom(src => src.InformationCertified))
                .ForMember(dest => dest.InterpreterLanguage, opt => opt.MapFrom(src => src.Additional == null ? null : src.Additional.InterpreterLanguage))
                .ForMember(dest => dest.InterpreterRequired, opt => opt.MapFrom(src => src.Additional == null ? false : src.Additional.InterpreterRequired))
                .ForMember(dest => dest.LawyerPresent, opt => opt.MapFrom(src => src.Additional == null ? null : src.Additional.LawyerPresent))
                .ForMember(dest => dest.ViolationTicketNumber, opt => opt.MapFrom(src => src.ViolationTicketNumber))
                .ForMember(dest => dest.WitnessPresent, opt => opt.MapFrom(src => src.Additional == null ? null : src.Additional.WitnessPresent))
                .ForMember(dest => dest.OffenceDisputeDetails, opt => opt.MapFrom<OffenceDisputeDetailsResolver>())
                ;

            CreateMap<Web.Models.OffenceDisputeDetail, DBModel.OffenceDisputeDetail>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.MoreTimeReason, opt => opt.MapFrom(src => src.MoreTimeReason))
                .ForMember(dest => dest.OffenceAgreementStatus, opt => opt.MapFrom(src => src.OffenceAgreementStatus))
                .ForMember(dest => dest.ReductionReason, opt => opt.MapFrom(src => src.ReductionReason))
                .ForMember(dest => dest.RequestMoreTime, opt => opt.MapFrom(src => src.RequestMoreTime))
                .ForMember(dest => dest.RequestReduction, opt => opt.MapFrom(src => src.RequestReduction))
                .ReverseMap()
                ;

            CreateMap<DBModel.Dispute, Web.Models.Disputant> ()
                .ForMember(dest => dest.GivenNames, opt => opt.MapFrom(src => src.DisputantFirstName))
                .ForMember(dest => dest.HomePhoneNumber, opt => opt.MapFrom(src => src.HomePhoneNumber))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.DisputantLastName))
                .ForMember(dest => dest.MailingAddress, opt => opt.MapFrom(src => src.DisputantMailingAddress))
                .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(src => src.DisputantMailingPostalCode))
                .ForMember(dest => dest.Province, opt => opt.MapFrom(src => src.DisputantMailingProvince))
                .ForMember(dest => dest.WorkPhoneNumber, opt => opt.MapFrom(src => src.WorkPhoneNumber))
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.DisputantBirthDate))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.DisputantMailingCity))
                .ForMember(dest => dest.DriverLicense, opt => opt.MapFrom(src => src.DriverLicense))
                .ForMember(dest => dest.DriverLicenseProvince, opt => opt.MapFrom(src => src.DriverLicenseProvince))
                .ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.DisputantEmailAddress))
                 ;

            CreateMap<DBModel.Dispute, Web.Models.Additional>()
                .ForMember(dest => dest.InterpreterLanguage, opt => opt.MapFrom(src => src.InterpreterLanguage))
                .ForMember(dest => dest.InterpreterRequired, opt => opt.MapFrom(src => src.InterpreterRequired))
                .ForMember(dest => dest.LawyerPresent, opt => opt.MapFrom(src => src.LawyerPresent))
                .ForMember(dest => dest.WitnessPresent, opt => opt.MapFrom(src => src.WitnessPresent))
                 ;

            CreateMap<DBModel.Dispute, DisputeContract>()
                ;

            CreateMap<DBModel.Dispute, GetDisputeResponse>();

        }
    }
}
