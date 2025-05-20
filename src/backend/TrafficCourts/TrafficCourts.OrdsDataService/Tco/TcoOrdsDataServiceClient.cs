namespace TrafficCourts.OrdsDataService.Tco
{
    internal class TcoOrdsDataServiceClient : OrdsDataServiceClient
    {
        public TcoOrdsDataServiceClient(HttpClient httpClient, IOrdsDataServiceOperationMetrics metrics) : base(httpClient, metrics)
        {
        }
    }
}
