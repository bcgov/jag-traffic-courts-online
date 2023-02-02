using System.ComponentModel.DataAnnotations;
using TrafficCourts.Common.Configuration.Validation;

namespace TrafficCourts.Messaging.Configuration;

/// <summary>
/// The host options for configuring RabbitMq. This aligns with properties from <see cref="MassTransit.RabbitMqHostSettings"/>.
/// </summary>
public class RabbitMqHostOptions : IValidatable
{
    /// <summary>
    /// The section to load the configuration from.
    /// </summary>
    public const string Section = "RabbitMq";

    /// <summary>
    /// The RabbitMQ host to connect to (should be a valid hostname)
    /// </summary>
    [Required]
    public string? Host { get; set; } = "localhost";

    /// <summary>
    /// The RabbitMQ port to connect, defaults to 5672
    /// </summary>
    public ushort Port { get; set; } = 5672;

    /// <summary>
    /// The Username for connecting to the host
    /// </summary>
    [Required]
    public string? Username { get; set; } = "guest";

    /// <summary>
    /// The password for connection to the host
    /// </summary>
    [Required]
    public string? Password { get; set; } = "guest";

    /// <summary>
    /// The client-provided name for the connection (displayed in RabbitMQ admin panel)
    /// </summary>
    public string? ClientProvidedName { get; set; }

    /// <summary>
    /// The virtual host for the connection
    /// </summary>
    public string VirtualHost { get; set; } = "/";

    public RetryOptions Retry { get; set; } = new RetryOptions();

    public void Validate()
    {
        if (string.IsNullOrEmpty(Host)) throw new SettingsValidationException(Section, nameof(Host), "is required");
        if (string.IsNullOrEmpty(Username)) throw new SettingsValidationException(Section, nameof(Username), "is required");
        if (string.IsNullOrEmpty(Password)) throw new SettingsValidationException(Section, nameof(Password), "is required");
        if (string.IsNullOrEmpty(VirtualHost)) throw new SettingsValidationException(Section, nameof(VirtualHost), "is required");

        if (Retry is null) throw new SettingsValidationException(Section, nameof(Retry), "is required");
    }
}
