using TrafficCourts.Common.Configuration;
using TrafficCourts.Messaging.Configuration;

namespace TrafficCourts.Citizen.Service.Configuration;

/// <summary>
/// The citizen service configuration.
/// </summary>
public class CitizenServiceConfiguration : 
    IRabbitMQConfiguration, 
    IMassTransitConfiguration,
    ITicketSearchServiceConfiguration,
    ISplunkConfiguration
{
    public RabbitMQConfigurationProperties? RabbitMQ { get; set; }
    public MassTransitConfigurationProperties? MassTransit { get; set; }
    public FormRecognizerConfigurationOptions? FormRecognizer { get; set; }
    public TicketSearchServiceConfigurationProperties? TicketSearchClient { get; set; }
    public SplunkConfigurationProperties? Splunk { get; set; }
    public FlatFileLookupServiceConfiguration? FlatFileLookupService { get; set; }
}
