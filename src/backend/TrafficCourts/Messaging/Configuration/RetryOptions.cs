namespace TrafficCourts.Messaging.Configuration;

public class RetryOptions
{
    /// <summary>
    /// The number of retry attempts
    /// </summary>
    public int Times { get; set; } = 5;

    /// <summary>
    /// The interval between each retry attempt
    /// </summary>
    public int Interval { get; set; } = 2;

    /// <summary>
    /// Limits the number of concurrent messages consumed on the receive endpoint, regardless of message type.
    /// </summary>
    public int ConcurrencyLimit { get; set; } = 2;
}