namespace TrafficCourts.Citizen.Service.Configuration;

public interface ITicketSearchServiceConfiguration
{
    TicketSearchServiceConfigurationProperties? TicketSearchClient { get; set; }
}