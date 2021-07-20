using AutoMapper;
using System.Diagnostics.CodeAnalysis;
using Xunit;
using TrafficCourts.Common;
using DisputeApi.Web.Features.Tickets.Mapping;
using AutoFixture;
using Gov.TicketSearch;
using DisputeApi.Web.Models;
using DisputeOffence = DisputeApi.Web.Models.Offence;
using TicketSearchOffence = Gov.TicketSearch.Offence;

namespace DisputeApi.Web.Test.Features.Tickets.Mapping
{
    [ExcludeFromCodeCoverage(Justification = Justifications.UnitTestClass)]
    public class MappingProfileTest
    {
        private IMapper _mapper;
        private Fixture _fixture;

        public MappingProfileTest()
        {
            var config = new MapperConfiguration(cfg => { cfg.AddProfile(new MappingProfile()); });
            _mapper = config.CreateMapper();
            _fixture = new Fixture();
        }

        [Fact]
        public void TicketSearchResponse_should_map_to_TicketDispute_correctly()
        {
            TicketSearchResponse response = _fixture.Create<TicketSearchResponse>();
            TicketDispute ticketDispute = _mapper.Map<TicketDispute>(response);
            Assert.Equal(response.ViolationTicketNumber, ticketDispute.ViolationTicketNumber);
            Assert.Equal(response.ViolationTime, ticketDispute.ViolationTime);
            Assert.Equal(response.ViolationDate, ticketDispute.ViolationDate);
            Assert.Equal(response.Offences.Count, ticketDispute.Offences.Count);
        }

        [Fact]
        public void TicketSearchOffence_should_map_to_DisputeOffence_correctly()
        {
            TicketSearchOffence searchOffence = _fixture.Create<TicketSearchOffence>();
            DisputeOffence disputeOffence = _mapper.Map<DisputeOffence>(searchOffence);
            Assert.Equal(searchOffence.AmountDue.ToString(), disputeOffence.AmountDue.ToString());
            Assert.Equal(searchOffence.OffenceNumber, disputeOffence.OffenceNumber);
            Assert.Equal(searchOffence.OffenceDescription, disputeOffence.OffenceDescription);
            Assert.Equal(searchOffence.VehicleDescription, disputeOffence.VehicleDescription);
        }
    }
}
