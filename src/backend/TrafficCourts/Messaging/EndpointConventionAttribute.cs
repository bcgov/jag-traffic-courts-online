namespace TrafficCourts.Messaging;

/// <summary>
/// Provides the ability to provide a standard endpoint convention on a message type.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class EndpointConventionAttribute : Attribute
{
    public string Name { get; init; }

    /// <summary>
    /// The endppoint name to use
    /// </summary>
    /// <param name="name"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public EndpointConventionAttribute(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

        Name = name.Trim();
    }
}
