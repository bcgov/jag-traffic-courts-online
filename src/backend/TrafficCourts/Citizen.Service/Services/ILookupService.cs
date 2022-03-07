using TrafficCourts.Citizen.Service.Models.Tickets;

namespace TrafficCourts.Citizen.Service.Services;

public interface ILookupService
{
    public IEnumerable<Statute> GetStatutes();
}
