using System.Text.Json.Serialization;
using System.Text.Json;

namespace DotNetThoughts.Results.Json;

public class JsonConverterForIError : JsonConverter<IError>
{
    public override IError Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return JsonSerializer.Deserialize<DeserializedError>(ref reader, options)!;
    }
    public override void Write(Utf8JsonWriter writer, IError value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, new
        {
            value.Type,
            value.Message,
            value.Data
        }, options);
    }
}
