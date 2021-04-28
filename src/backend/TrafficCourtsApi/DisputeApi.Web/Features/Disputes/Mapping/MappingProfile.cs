using AutoMapper;
using DisputeApi.Web.Features.Disputes.Commands;
using DisputeApi.Web.Features.Disputes.Queries;
using ContractDispute = TrafficCourts.Common.Contract.Dispute;

namespace DisputeApi.Web.Features.Disputes.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateDisputeCommand, DBModel.Dispute>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => TrafficCourts.Common.Contract.DisputeStatus.Submitted))
                ;

            CreateMap<CreateDisputeCommand, ContractDispute>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => TrafficCourts.Common.Contract.DisputeStatus.Submitted))
                ;

            CreateMap<DBModel.Dispute, GetDisputeResponse>();

        }
    }
}
