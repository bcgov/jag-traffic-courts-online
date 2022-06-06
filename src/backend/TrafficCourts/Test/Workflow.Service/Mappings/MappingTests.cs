using AutoFixture;
using AutoMapper;
using System;
using System.Collections.Generic;
using TrafficCourts.Arc.Dispute.Service.Mappings;
using TrafficCourts.Arc.Dispute.Service.Models;
using TrafficCourts.Common;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Workflow.Service.Mappings;
using Xunit;
using DisputedCount = TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.DisputedCount;
using ViolationTicketCount = TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.ViolationTicketCount;

namespace TrafficCourts.Test.Workflow.Service.Mappings;

public class MappingTests
{
    [Fact]
    public void TestMessageContractToDisputeMappingProfile()
    {
        // Arrange
        var fixture = new Fixture();
        fixture.Register<DateOnly>(() => new(2000, 1, 31));
        var source = fixture.Create<SubmitNoticeOfDispute>();

        var mockMapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MessageContractToDisputeMappingProfile());
        });
        var mapper = mockMapper.CreateMapper();

        // Act
        var target = mapper.Map<Dispute>(source);

        // Assert
        Assert.NotNull(target);

        Assert.Equal(source.Status.ToString().ToUpper(), target.Status.ToString().ToUpper());
        Assert.Equal(source.TicketNumber, target.TicketNumber);
        Assert.Equal(source.ProvincialCourtHearingLocation, target.ProvincialCourtHearingLocation);
        Assert.Equal(source.IssuedDate, target.IssuedDate);
        Assert.Equal(source.SubmittedDate, target.SubmittedDate);
        Assert.Equal(source.Surname, target.Surname);
        Assert.Equal(source.GivenNames, target.GivenNames);
        Assert.Equal(source.Birthdate, target.Birthdate);
        Assert.Equal(source.DriversLicenceNumber, target.DriversLicenceNumber);
        Assert.Equal(source.DriversLicenceProvince, target.DriversLicenceProvince);
        Assert.Equal(source.Address, target.Address);
        Assert.Equal(source.City, target.City);
        Assert.Equal(source.Province, target.Province);
        Assert.Equal(source.PostalCode, target.PostalCode);
        Assert.Equal(source.HomePhoneNumber, target.HomePhoneNumber);
        Assert.Equal(source.WorkPhoneNumber, target.WorkPhoneNumber);
        Assert.Equal(source.EmailAddress, target.EmailAddress);
        Assert.Equal(source.RepresentedByLawyer, target.RepresentedByLawyer);
        Assert.Equal(source.InterpreterLanguage, target.InterpreterLanguage);
        Assert.Equal(source.NumberOfWitness, target.NumberOfWitness);
        Assert.Equal(source.FineReductionReason, target.FineReductionReason);
        Assert.Equal(source.TimeToPayReason, target.TimeToPayReason);
        Assert.Equal(source.DisputantDetectedOcrIssues, target.DisputantDetectedOcrIssues);
        Assert.Equal(source.DisputantOcrIssuesDescription, target.DisputantOcrIssuesDescription);
        Assert.Equal(source.OcrViolationTicket, target.OcrViolationTicket);

        Assert.Equal(source.ViolationTicket?.TicketNumber, target.ViolationTicket.TicketNumber);
        Assert.Equal(source.ViolationTicket?.Surname, target.ViolationTicket.Surname);
        Assert.Equal(source.ViolationTicket?.GivenNames, target.ViolationTicket.GivenNames);
        Assert.Equal(source.ViolationTicket?.DriversLicenceNumber, target.ViolationTicket.DriversLicenceNumber);
        Assert.Equal(source.ViolationTicket?.DriversLicenceProvince, target.ViolationTicket.DriversLicenceProvince);
        Assert.Equal(source.ViolationTicket?.Birthdate.Year, target.ViolationTicket.Birthdate?.Year);
        Assert.Equal(source.ViolationTicket?.Address, target.ViolationTicket.Address);
        Assert.Equal(source.ViolationTicket?.City, target.ViolationTicket.City);
        Assert.Equal(source.ViolationTicket?.Province, target.ViolationTicket.Province);
        Assert.Equal(source.ViolationTicket?.PostalCode, target.ViolationTicket.PostalCode);
        Assert.Equal(source.ViolationTicket?.IssuedDate, target.ViolationTicket.IssuedDate);
        Assert.Equal(source.ViolationTicket?.OrganizationLocation, target.ViolationTicket.OrganizationLocation);
        Assert.Equal(source.ViolationTicket?.ViolationTicketCounts.Count, target.ViolationTicket.ViolationTicketCounts.Count);
        List<ViolationTicketCount> ticketCounts = new(target.ViolationTicket.ViolationTicketCounts);
        for (int i = 0; i < source.ViolationTicket?.ViolationTicketCounts.Count; i++)
        {
            Assert.Equal(source.ViolationTicket?.ViolationTicketCounts[i].Count.ToString(), ticketCounts[i].Count.ToString());
            Assert.Equal(source.ViolationTicket?.ViolationTicketCounts[i].Description, ticketCounts[i].Description);
            Assert.Equal(source.ViolationTicket?.ViolationTicketCounts[i].FullSection, ticketCounts[i].FullSection);
            Assert.Equal(source.ViolationTicket?.ViolationTicketCounts[i].ActRegulation, ticketCounts[i].ActRegulation);
            Assert.Equal(source.ViolationTicket?.ViolationTicketCounts[i].TicketedAmount, ticketCounts[i].TicketedAmount);
            Assert.Equal(source.ViolationTicket?.ViolationTicketCounts[i].IsAct, ticketCounts[i].IsAct);
            Assert.Equal(source.ViolationTicket?.ViolationTicketCounts[i].IsRegulation, ticketCounts[i].IsRegulation);
        }

        Assert.Equal(source.LegalRepresentation?.LawFirmName, target.LegalRepresentation.LawFirmName);
        Assert.Equal(source.LegalRepresentation?.LawyerFullName, target.LegalRepresentation.LawyerFullName);
        Assert.Equal(source.LegalRepresentation?.LawyerEmail, target.LegalRepresentation.LawyerEmail);
        Assert.Equal(source.LegalRepresentation?.LawyerAddress, target.LegalRepresentation.LawyerAddress);
        Assert.Equal(source.LegalRepresentation?.LawyerPhoneNumber, target.LegalRepresentation.LawyerPhoneNumber);

        Assert.Equal(source.DisputedCounts.Count, target.DisputedCounts.Count);
        List<DisputedCount> disputedCounts = new(target.DisputedCounts);
        for (int i = 0; i < source.DisputedCounts?.Count; i++)
        {
            Assert.Equal(source.DisputedCounts?[i].Plea.GetTypeCode(), disputedCounts[i].Plea.GetTypeCode());
            Assert.Equal(source.DisputedCounts?[i].Count, disputedCounts[i].Count);
            Assert.Equal(source.DisputedCounts?[i].RequestTimeToPay, disputedCounts[i].RequestTimeToPay);
            Assert.Equal(source.DisputedCounts?[i].RequestReduction, disputedCounts[i].RequestReduction);
            Assert.Equal(source.DisputedCounts?[i].AppearInCourt, disputedCounts[i].AppearInCourt);
        }
    }
}
