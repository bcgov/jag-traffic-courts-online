using HashidsNet;
using TrafficCourts.Citizen.Service.Configuration;

namespace TrafficCourts.Citizen.Service.Services.Impl
{
    /// <summary>
    /// Implentation of the IHashingidsService that is used for saving and retrieving json serialized generic data to/from Redis.
    /// </summary>
    public class HashidsService : IHashidsService
    {
        private readonly ILogger<HashidsService> _logger;
        private Hashids hashids = new Hashids("");

        public HashidsService(ILogger<HashidsService> logger, HashidsOptions options)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            ArgumentNullException.ThrowIfNull(options);
            this.hashids = new Hashids(options.Salt);
        }

        public Hashids GetHashids()
        {
            return this.hashids;
        }

    }
}
