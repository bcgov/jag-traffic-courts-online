using AutoMapper;
using System.Diagnostics.CodeAnalysis;
using Xunit;
using Gov.CitizenApi.Features.Tickets.Mapping;
using AutoFixture;
using Gov.TicketSearch;
using Gov.CitizenApi.Models;
using DisputeOffence = Gov.CitizenApi.Models.Offence;
using TicketSearchOffence = Gov.TicketSearch.Offence;
using Gov.CitizenApi.Features.Tickets.Commands;
using Gov.CitizenApi.Features.Tickets.DBModel;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Gov.CitizenApi.Features.Lookups;
using Moq;
using System;

namespace Gov.CitizenApi.Test.Features.Tickets.Mapping
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

        [Fact]
        public void CreateShellTicketCommand_should_map_to_Ticket_correctly()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            Mock<ILookupsService> lookupsMock = new Mock<ILookupsService>();
            lookupsMock.Setup(m => m.GetCountStatute(It.IsAny<decimal>())).Returns(new Statute { Code=111m, Name="codeDesc"});
            serviceCollection.AddTransient<ILookupsService>(m => lookupsMock.Object);
            serviceCollection.AddAutoMapper(typeof(MappingProfile));

            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
            IMapper mapperStub = serviceProvider.GetService<IMapper>();
            CreateShellTicketCommand command = _fixture.Create<CreateShellTicketCommand>();
            command.Count1Charge = 11817;
            command.Count2Charge = 11817;
            command.Count3Charge = 11817;
            Ticket ticket = mapperStub.Map<Ticket>(command);
            Assert.Equal(ticket.DriverLicenseNumber, command.DriverLicenseNumber);
            Assert.Equal(ticket.ViolationTicketNumber, command.ViolationTicketNumber);
            Assert.Equal(command.Count1FineAmount, ticket.Offences.First().AmountDue);
            Assert.Equal(command.Count1Charge, ticket.Offences.First().OffenceCode);
            Assert.Equal("codeDesc", ticket.Offences.First().OffenceDescription);
        }
    }
}
