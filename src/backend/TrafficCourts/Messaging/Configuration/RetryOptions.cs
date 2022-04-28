namespace TrafficCourts.Messaging.Configuration;

public class RetryOptions
{
    /// <summary>
    /// Retry Times
    /// </summary>
    public int Times { get; set; } = 5;

    /// <summary>
    /// Re Try Interval 
    /// </summary>
    public int Interval { get; set; } = 2;

    /// <summary>
    /// Concurrency limit for consumer
    /// </summary>
    public int ConcurrencyLimit { get; set; } = 2;
}