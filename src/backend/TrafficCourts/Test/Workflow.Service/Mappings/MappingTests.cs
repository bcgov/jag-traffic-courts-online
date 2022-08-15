﻿using AutoFixture;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using TrafficCourts.Arc.Dispute.Service.Mappings;
using TrafficCourts.Arc.Dispute.Service.Models;
using TrafficCourts.Common;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Workflow.Service.Mappings;
using Xunit;
using DisputeCount = TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.DisputeCount;
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
        Assert.Equal(source.CourtLocation, target.CourtLocation);
        Assert.Equal(source.IssuedDate, target.IssuedDate);
        Assert.Equal(source.SubmittedDate, target.SubmittedDate);
        Assert.Equal(source.DisputantSurname, target.DisputantSurname);
        Assert.Equal(source.DisputantGivenName1, target.DisputantGivenName1);
        Assert.Equal(source.DisputantGivenName2, target.DisputantGivenName2);
        Assert.Equal(source.DisputantGivenName3, target.DisputantGivenName3);
        Assert.Equal(source.DisputantBirthdate, target.DisputantBirthdate);
        Assert.Equal(source.DriversLicenceNumber, target.DriversLicenceNumber);
        Assert.Equal(source.DriversLicenceProvince, target.DriversLicenceProvince);
        Assert.Equal(source.Address, target.Address);
        Assert.Equal(source.AddressCity, target.AddressCity);
        Assert.Equal(source.AddressProvince, target.AddressProvince);
        Assert.Equal(source.PostalCode, target.PostalCode);
        Assert.Equal(source.HomePhoneNumber, target.HomePhoneNumber);
        Assert.Equal(source.WorkPhoneNumber, target.WorkPhoneNumber);
        Assert.Equal(source.EmailAddress, target.EmailAddress);
        Assert.Equal(source.RepresentedByLawyer, target.RepresentedByLawyer);
        Assert.Equal(source.InterpreterLanguage, target.InterpreterLanguage);
        Assert.Equal(source.WitnessNo, target.WitnessNo);
        Assert.Equal(source.FineReductionReason, target.FineReductionReason);
        Assert.Equal(source.TimeToPayReason, target.TimeToPayReason);
        Assert.Equal(source.DisputantDetectedOcrIssues, target.DisputantDetectedOcrIssues);
        Assert.Equal(source.DisputantOcrIssues, target.DisputantOcrIssues);
        Assert.Equal(source.OcrViolationTicket, target.OcrViolationTicket);

        Assert.Equal(source.ViolationTicket?.TicketNumber, target.ViolationTicket.TicketNumber);
        Assert.Equal(source.ViolationTicket?.DisputantSurname, target.ViolationTicket.DisputantSurname);
        Assert.Equal(source.ViolationTicket?.DisputantGivenNames, target.ViolationTicket.DisputantGivenNames);
        Assert.Equal(source.ViolationTicket?.DisputantDriversLicenceNumber, target.ViolationTicket.DisputantDriversLicenceNumber);
        Assert.Equal(source.ViolationTicket?.DriversLicenceProvince, target.ViolationTicket.DriversLicenceProvince);
        Assert.Equal(source.ViolationTicket?.DisputantBirthdate.Year, target.ViolationTicket.DisputantBirthdate?.Year);
        Assert.Equal(source.ViolationTicket?.Address, target.ViolationTicket.Address);
        Assert.Equal(source.ViolationTicket?.AddressCity, target.ViolationTicket.AddressCity);
        Assert.Equal(source.ViolationTicket?.AddressProvince, target.ViolationTicket.AddressProvince);
        Assert.Equal(source.ViolationTicket?.AddressPostalCode, target.ViolationTicket.AddressPostalCode);
        Assert.Equal(source.ViolationTicket?.IssuedDate, target.ViolationTicket.IssuedDate);
        Assert.Equal(source.ViolationTicket?.DetatchmentLocation, target.ViolationTicket.DetachmentLocation);
        Assert.Equal(source.ViolationTicket?.ViolationTicketCounts.Count, target.ViolationTicket.ViolationTicketCounts.Count);
        List<ViolationTicketCount> ticketCounts = new(target.ViolationTicket.ViolationTicketCounts);
        for (int i = 0; i < source.ViolationTicket?.ViolationTicketCounts.Count; i++)
        {
            Assert.Equal(source.ViolationTicket?.ViolationTicketCounts[i].CountNo.ToString(), ticketCounts[i].CountNo.ToString());
            Assert.Equal(source.ViolationTicket?.ViolationTicketCounts[i].Description, ticketCounts[i].Description);
            Assert.Equal(source.ViolationTicket?.ViolationTicketCounts[i].FullSection, ticketCounts[i].FullSection);
            Assert.Equal(source.ViolationTicket?.ViolationTicketCounts[i].ActOrRegulationNameCode, ticketCounts[i].ActOrRegulationNameCode);
            Assert.Equal(source.ViolationTicket?.ViolationTicketCounts[i].TicketedAmount, ticketCounts[i].TicketedAmount);
            Assert.Equal(source.ViolationTicket?.ViolationTicketCounts[i].IsAct, ticketCounts[i].IsAct);
            Assert.Equal(source.ViolationTicket?.ViolationTicketCounts[i].IsRegulation, ticketCounts[i].IsRegulation);
            Assert.Equal(source.ViolationTicket?.ViolationTicketCounts[i].DisputeCount?.PleaCode, ticketCounts[i].DisputeCount?.PleaCode);
            Assert.Equal(source.ViolationTicket?.ViolationTicketCounts[i].DisputeCount?.RequestCourtAppearance, ticketCounts[i].DisputeCount?.RequestCourtAppearance);
            Assert.Equal(source.ViolationTicket?.ViolationTicketCounts[i].DisputeCount?.RequestReduction, ticketCounts[i].DisputeCount?.RequestReduction);
            Assert.Equal(source.ViolationTicket?.ViolationTicketCounts[i].DisputeCount?.RequestTimeToPay, ticketCounts[i].DisputeCount?.RequestTimeToPay);
        }

        Assert.Equal(source.LawFirmName, target.LawFirmName);
        Assert.Equal(source.LawyerSurname, target.LawyerSurname);
        Assert.Equal(source.LawyerGivenName1, target.LawyerGivenName1);
        Assert.Equal(source.LawyerGivenName2, target.LawyerGivenName2);
        Assert.Equal(source.LawyerEmail, target.LawyerEmail);
        Assert.Equal(source.LawyerAddress, target.LawyerAddress);
        Assert.Equal(source.LawyerPhoneNumber, target.LawyerPhoneNumber);
    }
}
