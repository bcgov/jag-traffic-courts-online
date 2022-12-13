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
        [InlineData(1, DisputeCountPleaCode.G)]
        [InlineData(1, DisputeCountPleaCode.N)]
        [InlineData(2, DisputeCountPleaCode.G)]
        [InlineData(2, DisputeCountPleaCode.N)]
        [InlineData(3, DisputeCountPleaCode.G)]
        [InlineData(3, DisputeCountPleaCode.N)]
        public void dispute_approve_maps_disputed_count_and_pleas(int count, DisputeCountPleaCode pleaCode)
        {
            Dispute source = new Dispute()
            {
                ViolationTicket = new ViolationTicket(),
                DisputeCounts = new List<DisputeCount>()
                {
                    new DisputeCount { CountNo = count, PleaCode = pleaCode }
                }
            };

            var actual = Mapper.ToDisputeApproved(source);
            Assert.NotNull(actual);

            var disputeCount = Assert.Single(actual.DisputeCounts);
            Assert.Equal(pleaCode.ToString(), disputeCount.DisputeType);
        }

        [Fact]
        public void ToDisputeApproved_throws_ArgumentNullException()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => Mapper.ToDisputeApproved(null!));
            Assert.NotNull(actual);
        }
    }
}
