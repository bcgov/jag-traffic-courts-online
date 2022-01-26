namespace TrafficCourts.Citizen.Service.Configuration;

public class TicketSearchServiceConfigurationProperties
{
    /// <summary>
    /// 
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Determines if we are using a encrypted or non-encrypted channel.
    /// </summary>
    public bool Secure { get; set; } = true;
}
