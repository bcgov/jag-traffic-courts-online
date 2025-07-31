using Newtonsoft.Json;

namespace TrafficCourts.OracleDataApi.Client;

public sealed class AsDateOnlyJsonConverter : JsonConverter<DateTime?>
{
    private const string DateFormat = "yyyy-MM-dd";

    public override void WriteJson(JsonWriter writer, DateTime? value, Newtonsoft.Json.JsonSerializer serializer)
    {
        if (value.HasValue && value.Value != default(DateTime))
        {
            writer.WriteValue(value.Value.ToString(DateFormat));
        }
        else
        {
            writer.WriteNull();
        }
    }

    public override DateTime? ReadJson(JsonReader reader, Type objectType, DateTime? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
        {
            return null;
        }

        var dateValue = serializer.Deserialize<DateTime>(reader);
        // Check if the date is the default value (January 1, 0001)
        return dateValue.Year == default(DateTime).Year
            ? (DateTime?)null 
            : dateValue;
    }
}
