using System;
using System.Collections.Generic;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Staff.Service.Mappers;
using Xunit;

namespace TrafficCourts.Staff.Service.Test.Mappers
{
    public class MapperTest
    {
        [Theory]
        [MemberData(nameof(GetTestCases))]
        public void dispute_approve_maps_disputed_count_to_dispute_type(int count, DisputeCountRequestReduction reduction, DisputeCountRequestTimeToPay timeToPay, string disputeType)
        {
            Dispute source = new Dispute()
            {
                ViolationTicket = new ViolationTicket(),
                NoticeOfDisputeGuid = Guid.NewGuid().ToString("d"),
                DisputeCounts =
                [
                    new DisputeCount
                    {
                        CountNo = count,
                        RequestReduction = reduction,
                        RequestTimeToPay = timeToPay
                    }
                ]
            };

            var actual = Mapper.ToDisputeApproved(source);
            Assert.NotNull(actual);

            var disputeCount = Assert.Single(actual.DisputeCounts);
            Assert.Equal(disputeType, disputeCount.DisputeType);
        }

        [Fact]
        public void ToDisputeApproved_throws_ArgumentNullException()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => Mapper.ToDisputeApproved(null!));
            Assert.NotNull(actual);
        }


        public static IEnumerable<object[]> GetTestCases
        {
            get
            {
                for (int count = 1; count <= 3; count++)
                {
                    foreach (var reduction in Enum.GetValues<DisputeCountRequestReduction>())
                    {
                        foreach (var timeToPay in Enum.GetValues<DisputeCountRequestTimeToPay>())
                        {
                            // if either requesting reduction or time to pay, should map to F otherwise A
                            string disputeType = (reduction == DisputeCountRequestReduction.Y || timeToPay == DisputeCountRequestTimeToPay.Y)
                                ? "F"   // find
                                : "A"; // allegation

                            yield return new object[] { count, reduction, timeToPay, disputeType };
                        }
                    }
                }
            }
        }
    }
}
