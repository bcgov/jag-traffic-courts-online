using AutoFixture;
using AutoMapper;
using TrafficCourts.Citizen.Service.Mappings;
using TrafficCourts.Citizen.Service.Models.Disputes;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
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
            Assert.Equal(DisputeStatus.NEW, actual.Status);
            Assert.Equal(noticeOfDispute.IssuedTs, actual.IssuedTs);
            Assert.Equal(noticeOfDispute.DisputantSurname, actual.DisputantSurname);
            Assert.Equal(noticeOfDispute.DisputantGivenName1, actual.DisputantGivenName1);
            Assert.Equal(noticeOfDispute.DisputantGivenName2, actual.DisputantGivenName2);
            Assert.Equal(noticeOfDispute.DisputantGivenName3, actual.DisputantGivenName3);
            Assert.Equal(noticeOfDispute.DisputantBirthdate, actual.DisputantBirthdate);
            Assert.Equal(noticeOfDispute.DriversLicenceNumber, actual.DriversLicenceNumber);
            Assert.Equal(noticeOfDispute.DriversLicenceProvince, actual.DriversLicenceProvince);
            Assert.Equal(noticeOfDispute.DriversLicenceProvinceSeqNo, actual.DriversLicenceProvinceSeqNo);
            Assert.Equal(noticeOfDispute.DriversLicenceCountryId, actual.DriversLicenceCountryId);
            Assert.Equal(noticeOfDispute.AddressLine1, actual.AddressLine1);
            Assert.Equal(noticeOfDispute.AddressLine2, actual.AddressLine2);
            Assert.Equal(noticeOfDispute.AddressLine3, actual.AddressLine3);
            Assert.Equal(noticeOfDispute.AddressCity, actual.AddressCity);
            Assert.Equal(noticeOfDispute.AddressProvince, actual.AddressProvince);
            Assert.Equal(noticeOfDispute.AddressCountryId, actual.AddressCountryId);
            Assert.Equal(noticeOfDispute.AddressProvinceCountryId, actual.AddressProvinceCountryId);
            Assert.Equal(noticeOfDispute.AddressProvinceSeqNo, actual.AddressProvinceSeqNo);
            Assert.Equal(noticeOfDispute.PostalCode, actual.PostalCode);
            Assert.Equal(noticeOfDispute.HomePhoneNumber, actual.HomePhoneNumber);
            Assert.Equal(noticeOfDispute.WorkPhoneNumber, actual.WorkPhoneNumber);
            Assert.Equal(noticeOfDispute.EmailAddress, actual.EmailAddress);
            Assert.Equal(noticeOfDispute.RepresentedByLawyer, actual.RepresentedByLawyer);
            Assert.Equal(noticeOfDispute.InterpreterLanguageCd, actual.InterpreterLanguageCd);
            Assert.Equal(noticeOfDispute.WitnessNo, actual.WitnessNo);
            Assert.Equal(noticeOfDispute.FineReductionReason, actual.FineReductionReason);
            Assert.Equal(noticeOfDispute.TimeToPayReason, actual.TimeToPayReason);
            Assert.Equal(noticeOfDispute.DisputantDetectedOcrIssues, actual.DisputantDetectedOcrIssues);
            Assert.Equal(noticeOfDispute.DisputantOcrIssues, actual.DisputantOcrIssues);
            Assert.Equal(noticeOfDispute.LawFirmName, actual.LawFirmName);
            Assert.Equal(noticeOfDispute.LawyerAddress, actual.LawyerAddress);
            Assert.Equal(noticeOfDispute.LawyerEmail, actual.LawyerEmail);
            Assert.Equal(noticeOfDispute.LawyerSurname, actual.LawyerSurname);
            Assert.Equal(noticeOfDispute.LawyerGivenName1, actual.LawyerGivenName1);
            Assert.Equal(noticeOfDispute.LawyerGivenName2, actual.LawyerGivenName2);
            Assert.Equal(noticeOfDispute.LawyerGivenName3, actual.LawyerGivenName3);
            Assert.Equal(noticeOfDispute.LawyerPhoneNumber, actual.LawyerPhoneNumber);

            foreach (var count in actual.DisputeCounts)
            {
                Assert.Contains(noticeOfDispute.DisputeCounts!, _ => (int)_.PleaCode == (int)count.PleaCode);
                Assert.Contains(noticeOfDispute.DisputeCounts!, _ => _.CountNo == count.CountNo);
                Assert.Contains(noticeOfDispute.DisputeCounts!, _ => _.RequestCourtAppearance == count.RequestCourtAppearance);
                Assert.Contains(noticeOfDispute.DisputeCounts!, _ => _.RequestReduction == count.RequestReduction);
                Assert.Contains(noticeOfDispute.DisputeCounts!, _ => _.RequestTimeToPay == count.RequestTimeToPay);
            }

        }

        /// <summary>
        /// Checks to ensure each field is mapped correctly on <see cref="DisputedCount"/>
        /// </summary>
        [Fact]
        public void DisputedCount_is_mapped_correctly()
        {
            var fixture = new Fixture();
            var mapper = CreateMapper();

            var expected = fixture.Create<TrafficCourts.Citizen.Service.Models.Disputes.DisputeCount>();

            var actual = mapper.Map<Messaging.MessageContracts.DisputeCount>(expected);

            Assert.Equal(expected.PleaCode, actual.PleaCode);
            Assert.Equal(expected.RequestTimeToPay, actual.RequestTimeToPay);
            Assert.Equal(expected.RequestReduction, actual.RequestReduction);
            Assert.Equal(expected.RequestCourtAppearance, actual.RequestCourtAppearance);
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
