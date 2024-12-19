using Model = TrafficCourts.Domain.Models.Statute;

namespace TrafficCourts.Citizen.Service.Features.Lookups.Statutes;

public record Response
{
    public Response(IEnumerable<Model> items)
    {
        Items = items.ToList();
    }

    public IList<Model> Items { get; }
}
