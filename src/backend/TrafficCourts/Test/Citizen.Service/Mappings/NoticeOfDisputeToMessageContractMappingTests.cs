using AutoFixture;
using AutoMapper;
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
            var mapper = CreateMapper();
            var noticeOfDispute = fixture.Create<NoticeOfDispute>();

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
            Assert.Equal(noticeOfDispute.Birthdate, actual.Birthdate);
            Assert.Equal(noticeOfDispute.DriversLicenceNumber, actual.DriversLicenceNumber);
            Assert.Equal(noticeOfDispute.DriversLicenceProvince, actual.DriversLicenceProvince);
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


        /// <summary>
        /// Checks to ensure each field is mapped correctly on <see cref="LegalRepresentation"/>
        /// </summary>
        [Fact]
        public void LegalRepresentation_is_mapped_correctly()
        {
            var fixture = new Fixture();
            var mapper = CreateMapper();

            var expected = fixture.Create<TrafficCourts.Citizen.Service.Models.Dispute.LegalRepresentation>();

            var actual = mapper.Map<Messaging.MessageContracts.LegalRepresentation>(expected);

            Assert.Equal(expected.LawFirmName, actual.LawFirmName);
            Assert.Equal(expected.LawyerAddress, actual.LawyerAddress);
            Assert.Equal(expected.LawyerEmail, actual.LawyerEmail);
            Assert.Equal(expected.LawyerFullName, actual.LawyerFullName);
            Assert.Equal(expected.LawyerPhoneNumber, actual.LawyerPhoneNumber);
        }

        /// <summary>
        /// Checks to ensure each field is mapped correctly on <see cref="DisputedCount"/>
        /// </summary>
        [Fact]
        public void DisputedCount_is_mapped_correctly()
        {
            var fixture = new Fixture();
            var mapper = CreateMapper();

            var expected = fixture.Create<TrafficCourts.Citizen.Service.Models.Dispute.DisputedCount>();

            var actual = mapper.Map<Messaging.MessageContracts.DisputedCount>(expected);

            Assert.Equal(expected.Plea.ToString(), actual.Plea.ToString());
            Assert.Equal(expected.Count, actual.Count);
            Assert.Equal(expected.RequestTimeToPay, actual.RequestTimeToPay);
            Assert.Equal(expected.RequestReduction, actual.RequestReduction);
            Assert.Equal(expected.AppearInCourt, actual.AppearInCourt);
        }

        /// <summary>
        /// Creates mapping with profile <see cref="NoticeOfDisputeToMessageContractMappingProfile"/>.
        /// </summary>
        private static IMapper CreateMapper()
        {
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new NoticeOfDisputeToMessageContractMappingProfile());
            });
            var mapper = mockMapper.CreateMapper();

            return mapper;
        }
    }
}
