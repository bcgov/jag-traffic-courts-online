using Microsoft.Extensions.Logging;

namespace TrafficCourts.OrdsDataService.Occam
{
    internal class OccamOrdsRepository<TOrdsRepository> : OrdsRepository<TOrdsRepository>
        where TOrdsRepository : OrdsRepository<TOrdsRepository>
    {
        public OccamOrdsRepository(OrdsDataServiceClient client, string path, ILogger<TOrdsRepository> logger) : base(client, path, logger)
        {
        }
    }
}
