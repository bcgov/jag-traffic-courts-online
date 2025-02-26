using Model = TrafficCourts.Domain.Models.DisputeCaseFileSummary;

namespace TrafficCourts.Staff.Service.Features.Occam.Disputes;

public record Response
{
    public Response(IList<Model> items)
    {
        Items = items;
    }

    public IList<Model> Items { get; }
}
