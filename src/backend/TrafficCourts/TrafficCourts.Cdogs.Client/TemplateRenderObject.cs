namespace TrafficCourts.Cdogs.Client;

/// <summary>
/// Based on the generated <see cref="TemplateRenderObject"/> type.
/// </summary>
/// <typeparam name="T">The generic type of data</typeparam>
public partial class TemplateRenderObject<T> : TemplateObject<T>
{
    [Newtonsoft.Json.JsonProperty("template", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public Template Template { get; set; } = default!;
}