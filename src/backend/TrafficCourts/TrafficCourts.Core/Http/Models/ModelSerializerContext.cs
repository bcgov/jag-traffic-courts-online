using System.Text.Json.Serialization;

namespace TrafficCourts.Core.Http.Models;

[JsonSourceGenerationOptions(WriteIndented = false)]
[JsonSerializable(typeof(Token))]
internal partial class SerializerContext : JsonSerializerContext
{
}
