using AutoMapper;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Workflow.Service.Models;

namespace TrafficCourts.Workflow.Service.Mappings
{
    public class MessageContractToNoticeOfDisputeMappingProfile : Profile
    {
        public MessageContractToNoticeOfDisputeMappingProfile()
        {
            CreateMap<SubmitNoticeOfDispute, Common.OpenAPIs.OracleDataApi.v1_0.Dispute>();

            CreateMap<Messaging.MessageContracts.DisputedCount, Common.OpenAPIs.OracleDataApi.v1_0.DisputedCount>();
            CreateMap<Messaging.MessageContracts.DisputedCount, Models.DisputedCount>();

            CreateMap<Messaging.MessageContracts.ViolationTicket, Common.OpenAPIs.OracleDataApi.v1_0.ViolationTicket>();
            CreateMap<Messaging.MessageContracts.ViolationTicket, Models.ViolationTicket>();

            CreateMap<Messaging.MessageContracts.TicketCount, Common.OpenAPIs.OracleDataApi.v1_0.ViolationTicketCount>();
            CreateMap<Messaging.MessageContracts.TicketCount, Models.ViolationTicketCount>();

            CreateMap<Messaging.MessageContracts.LegalRepresentation, Common.OpenAPIs.OracleDataApi.v1_0.LegalRepresentation>();
            CreateMap<Messaging.MessageContracts.LegalRepresentation, Models.LegalRepresentation>();

            CreateMap<DateOnly, DateTime>();
        }
    }
}
