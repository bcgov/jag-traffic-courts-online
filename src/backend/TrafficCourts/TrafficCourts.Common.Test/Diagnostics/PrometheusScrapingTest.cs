using Microsoft.AspNetCore.Http;
using TrafficCourts.Common.Diagnostics;
using Xunit;

namespace TrafficCourts.Common.Test.Diagnostics
{
    public class PrometheusScrapingTest
    {
        [Theory]
        [InlineData("/metrics", 9090, true)]
        [InlineData("/prometheus", 9090, false)]
        [InlineData("/metrics", 8080, false)]
        public void should_filter_on_correct_path_and_port(string path, int localPort, bool expected)
        {
            HttpContext context = new DefaultHttpContext();
            context.Request.Path = path;
            context.Connection.LocalPort = localPort;

            // act
            var actual = PrometheusScraping.EndpointFilter(context);
            Assert.Equal(expected, actual);
        }
    }
}
