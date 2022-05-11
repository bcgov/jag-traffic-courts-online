using TrafficCourts.Common.Configuration.Validation;

namespace TrafficCourts.Common.Configuration;

public class TicketStorageConfiguration : IValidatable
{
    public const string Section = "TicketStorage";

    public TicketStorageType Type { get; set; } = TicketStorageType.InMemory;

    public void Validate()
    {
    }
}
