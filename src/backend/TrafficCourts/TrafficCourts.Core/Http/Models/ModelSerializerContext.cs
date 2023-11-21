using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace TrafficCourts.Core.Http.Models;

[ExcludeFromCodeCoverage]
[JsonSourceGenerationOptions(WriteIndented = false)]
[JsonSerializable(typeof(Token))]
internal partial class SerializerContext : JsonSerializerContext
{
}
