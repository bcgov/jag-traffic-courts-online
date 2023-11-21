using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace TrafficCourts.Coms.Client;

[ExcludeFromCodeCoverage]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(ObjectMetadataCollection))]
//[JsonSerializable(typeof(ObjectMetadata))]
//[JsonSerializable(typeof(MetadataItem))]
internal partial class ComsJsonContext : JsonSerializerContext
{
}
