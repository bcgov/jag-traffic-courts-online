using Microsoft.Extensions.Logging;

namespace TrafficCourts.OrdsDataService.Tco
{
    internal class TcoOrdsRepository<TOrdsRepository> : OrdsRepository<TOrdsRepository>
        where TOrdsRepository : OrdsRepository<TOrdsRepository>
    {
        public TcoOrdsRepository(OrdsDataServiceClient client, string path, ILogger<TOrdsRepository> logger) : base(client, path, logger)
        {
        }
    }
}
