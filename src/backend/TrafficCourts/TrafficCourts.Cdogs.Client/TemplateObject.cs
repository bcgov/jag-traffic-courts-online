namespace TrafficCourts.Cdogs.Client;

/// <summary>
/// Based on the generated <see cref="TemplateObject"/> type.
/// </summary>
/// <typeparam name="T">The generic type of data</typeparam>
public class TemplateObject<T>
{
    /// <summary>
    /// Template data
    /// </summary>
    [Newtonsoft.Json.JsonProperty("data", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public T Data { get; set; } = default!;

    /// <summary>
    /// A string that can be transformed into an object. See https://www.npmjs.com/package/telejson for transformations, and https://carbone.io/documentation.html#formatters for more on formatters.
    /// </summary>
    [Newtonsoft.Json.JsonProperty("formatters", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public string Formatters { get; set; } = default!;

    /// <summary>
    /// Object containing processing options
    /// </summary>
    [Newtonsoft.Json.JsonProperty("options", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public Options Options { get; set; } = default!;

    private System.Collections.Generic.IDictionary<string, object>? _additionalProperties;

    [Newtonsoft.Json.JsonExtensionData]
    public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
    {
        get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
        set { _additionalProperties = value; }
    }
}
