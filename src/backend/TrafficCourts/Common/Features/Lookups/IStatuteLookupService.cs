using TrafficCourts.Common.Models;

namespace TrafficCourts.Common.Features.Lookups
{
    public interface IStatuteLookupService : ICachedLookupService<Statute>
    {
        Task<Statute?> GetBySectionAsync(string section);
    }
}
