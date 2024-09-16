using System.Text.Json.Serialization;

namespace TrafficCourts.Staff.Service.Models;

/// <summary>
/// JSON source generator context
/// </summary>
[JsonSerializable(typeof(CountUpdateJSON))]
[JsonSerializable(typeof(DocumentUpdateJSON))]
public partial class ModelJsonSerializerContext : JsonSerializerContext
{
}
