using Model = TrafficCourts.Domain.Models.Language;

namespace TrafficCourts.Citizen.Service.Features.Lookups.Languages;

public record Response
{
    public Response(IList<Model> items)
    {
        Items = items;
    }

    public IList<Model> Items { get; }
}
