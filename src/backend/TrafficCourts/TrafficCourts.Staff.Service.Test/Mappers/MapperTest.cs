using AutoFixture;
using MassTransit;
using System;
using System.Collections.Generic;
using TrafficCourts.Domain.Models;
using TrafficCourts.Staff.Service.Mappers;
using Xunit;

namespace TrafficCourts.Staff.Service.Test.Mappers
{
    public class MapperTest
    {
        private readonly Fixture _fixture = new();
        private readonly Dispute _expected;

        public MapperTest()
        {
            // create object we will map.
            _expected = _fixture.Create<Dispute>();

            // oracle data api NoticeOfDisputeGuid is string, but internally should be a Guid
            _expected.NoticeOfDisputeGuid = Guid.NewGuid().ToString("d");
        }

        [Theory]
        [MemberData(nameof(GetTestCases))]
        public void dispute_approve_maps_disputed_count_to_dispute_type(int count, string disputeType, DisputeCountRequestCourtAppearance requestCourtAppearance, DisputeCountPleaCode plea)
        {
            Dispute expected = _expected;

            expected.DisputeCounts = [
                    new DisputeCount
                    {
                        CountNo = count,
                        RequestReduction = DisputeCountRequestReduction.Y,
                        RequestTimeToPay = DisputeCountRequestTimeToPay.Y,
                        RequestCourtAppearance = requestCourtAppearance,
                        PleaCode = plea
                    }
                ];

            var actual = Mapper.ToDisputeApproved(expected);
            Assert.NotNull(actual);

            var disputeCount = Assert.Single(actual.DisputeCounts);
            Assert.Equal(disputeType, disputeCount.DisputeType);
        }

        [Theory]
        [MemberData(nameof(GetTestCases))]
        public void dispute_approve_does_not_map_skipped_counts(int count, string disputeType, DisputeCountRequestCourtAppearance requestCourtAppearance, DisputeCountPleaCode plea)
        {
            Dispute expected = _expected;

            expected.DisputeCounts = [
                    new DisputeCount
                    {
                        CountNo = count,
                        RequestReduction = DisputeCountRequestReduction.N,
                        RequestTimeToPay = DisputeCountRequestTimeToPay.N,
                        RequestCourtAppearance = requestCourtAppearance,
                        PleaCode = plea
                    }
                ];

            var actual = Mapper.ToDisputeApproved(expected);
            if (requestCourtAppearance == DisputeCountRequestCourtAppearance.N)
            {
                Assert.Empty(actual.DisputeCounts);
            }
            else
            {
                var disputeCount = Assert.Single(actual.DisputeCounts);
                Assert.Equal(disputeType, disputeCount.DisputeType);
            }
        }

        [Fact]
        public void maps_issuing_organziation_and_location()
        {
            var actual = Mapper.ToDisputeApproved(_expected);
            Assert.NotNull(actual);

            // TCVP-2793 - issuing organziation should always be police (POL) and issuing location is the detachment location
            Assert.Equal("POL", actual.IssuingOrganization);
            Assert.Equal(_expected.ViolationTicket.DetachmentLocation, actual.IssuingLocation);
        }

        [Fact]
        public void maps_disputant_name()
        {
            var actual = Mapper.ToDisputeApproved(_expected);
            Assert.NotNull(actual);

            Assert.Equal(_expected.DisputantSurname, actual.Surname);
            Assert.Equal(_expected.DisputantGivenName1, actual.GivenName1);
            Assert.Equal(_expected.DisputantGivenName2, actual.GivenName2);
            Assert.Equal(_expected.DisputantGivenName3, actual.GivenName3);
        }

        [Fact]
        public void maps_ticket_number_and_time()
        {
            var actual = Mapper.ToDisputeApproved(_expected);
            Assert.NotNull(actual);

            Assert.Equal(_expected.TicketNumber, actual.TicketFileNumber);
            Assert.Equal(_expected.IssuedTs, actual.TicketIssuanceDate);
        }

        [Fact]
        public void ToDisputeApproved_throws_ArgumentNullException()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => Mapper.ToDisputeApproved(null!));
            Assert.Equal("dispute", actual.ParamName);
        }


        public static IEnumerable<object[]> GetTestCases
        {
            get
            {
                for (int count = 1; count <= 3; count++)
                {
                    foreach (var requestCourtAppearance in Enum.GetValues<DisputeCountRequestCourtAppearance>())
                    {
                        foreach (var plea in Enum.GetValues<DisputeCountPleaCode>())
                        {
                            // if either requesting reduction or time to pay, should map to F otherwise A
                            string disputeType = (requestCourtAppearance == DisputeCountRequestCourtAppearance.N ||
                                (requestCourtAppearance == DisputeCountRequestCourtAppearance.Y && plea == DisputeCountPleaCode.G))
                                ? "F"   // find
                                : "A"; // allegation

                            yield return new object[] { count, disputeType, requestCourtAppearance, plea};
                        }
                    }
                }
            }
        }
    }
}
