namespace TrafficCourts.OrdsDataService.Occam
{
    internal class OccamOrdsDataServiceClient : OrdsDataServiceClient
    {
        public OccamOrdsDataServiceClient(HttpClient httpClient, IOrdsDataServiceOperationMetrics metrics) : base(httpClient, metrics)
        {
        }
    }
}
