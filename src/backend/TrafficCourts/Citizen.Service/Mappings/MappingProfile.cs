﻿using AutoMapper;
using TrafficCourts.Citizen.Service.Models.Dispute;
using TrafficCourts.Messaging.MessageContracts;

namespace TrafficCourts.Citizen.Service.Mappings
{
    public class NoticeOfDisputeToMessageContractMappingProfile : Profile
    {
        public NoticeOfDisputeToMessageContractMappingProfile()
        {
            CreateMap<NoticeOfDispute, SubmitNoticeOfDispute>();
            CreateMap<Models.Dispute.DisputeCount, Messaging.MessageContracts.DisputeCount>();
            CreateMap<Models.Tickets.ViolationTicket, Messaging.MessageContracts.ViolationTicket>()
                .ForMember(dest => dest.ViolationTicketCounts, opt => opt.MapFrom(src => src.Counts));
            CreateMap<Models.Tickets.ViolationTicketCount, Messaging.MessageContracts.TicketCount>()
                .ForMember(dest => dest.FullSection, opt => opt.MapFrom(src => src.Section));
        }
    }
}
