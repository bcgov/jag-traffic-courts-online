using AutoFixture;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficCourts.Citizen.Service.Mappings;
using TrafficCourts.Citizen.Service.Models.Dispute;
using TrafficCourts.Messaging.MessageContracts;
using Xunit;

namespace TrafficCourts.Test.Citizen.Service.Mappings
{
    public class NoticeOfDisputeToMessageContractMappingTests
    {
        [Fact]
        public void TestMapDataFromNoticeOfDisputeToMessageContract()
        {
            // Arrange
            var fixture = new Fixture();
            var noticeOfDispute = fixture.Create<NoticeOfDispute>();

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new NoticeOfDisputeToMessageContractMappingProfile());
            });
            var mapper = mockMapper.CreateMapper();

            // Act
            var actual = mapper.Map<SubmitNoticeOfDispute>(noticeOfDispute);

            // Assert
            Assert.NotNull(actual);

            Assert.Equal(noticeOfDispute.TicketNumber, actual.TicketNumber);
            Assert.Equal(DisputeStatus.New, actual.Status);
            Assert.Equal(noticeOfDispute.ProvincialCourtHearingLocation, actual.ProvincialCourtHearingLocation);
            Assert.Equal(noticeOfDispute.IssuedDate, actual.IssuedDate);
            Assert.Equal(noticeOfDispute.Surname, actual.Surname);
            Assert.Equal(noticeOfDispute.GivenNames, actual.GivenNames);
            Assert.Equal(noticeOfDispute.Address, actual.Address);
            Assert.Equal(noticeOfDispute.City, actual.City);
            Assert.Equal(noticeOfDispute.Province, actual.Province);
            Assert.Equal(noticeOfDispute.PostalCode, actual.PostalCode);
            Assert.Equal(noticeOfDispute.HomePhoneNumber, actual.HomePhoneNumber);
            Assert.Equal(noticeOfDispute.WorkPhoneNumber, actual.WorkPhoneNumber);
            Assert.Equal(noticeOfDispute.EmailAddress, actual.EmailAddress);
            Assert.Equal(noticeOfDispute.RepresentedByLawyer, actual.RepresentedByLawyer);
            Assert.Equal(noticeOfDispute.InterpreterLanguage, actual.InterpreterLanguage);
            Assert.Equal(noticeOfDispute.NumberOfWitness, actual.NumberOfWitness);
            Assert.Equal(noticeOfDispute.FineReductionReason, actual.FineReductionReason);
            Assert.Equal(noticeOfDispute.TimeToPayReason, actual.TimeToPayReason);
            Assert.Equal(noticeOfDispute.CitizenDetectedOcrIssues, actual.CitizenDetectedOcrIssues);
            Assert.Equal(noticeOfDispute.CitizenOcrIssuesDescription, actual.CitizenOcrIssuesDescription);

            foreach (var count in actual.DisputedCounts)
            {
                Assert.Contains(noticeOfDispute.DisputedCounts, _ => (int)_.Plea == (int)count.Plea);
                Assert.Contains(noticeOfDispute.DisputedCounts, _ => _.Count == count.Count);
                Assert.Contains(noticeOfDispute.DisputedCounts, _ => _.AppearInCourt == count.AppearInCourt);
                Assert.Contains(noticeOfDispute.DisputedCounts, _ => _.RequestReduction == count.RequestReduction);
                Assert.Contains(noticeOfDispute.DisputedCounts, _ => _.RequestTimeToPay == count.RequestTimeToPay);
            }

            Assert.NotNull(actual.LegalRepresentation);
        }
    }
}
