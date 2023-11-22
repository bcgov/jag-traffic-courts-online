using System.Text.Json.Serialization;

namespace TrafficCourts.Coms.Client;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(ObjectMetadataCollection))]
internal partial class ComsJsonContext : JsonSerializerContext
{
}
