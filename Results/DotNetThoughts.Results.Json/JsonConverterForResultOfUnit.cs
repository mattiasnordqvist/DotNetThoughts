using System.Text.Json.Serialization;
using System.Text.Json;

namespace DotNetThoughts.Results.Json;
internal class JsonConverterForResultOfUnit : JsonConverter<Result<Unit>>
{
    public override Result<Unit> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        bool success = false;
        List<DeserializedError> errors = [];
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return success
                    ? UnitResult.Ok
                    : UnitResult.Error(errors);
            }
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }
            string propertyName = reader.GetString()!.ToLower();
            if (propertyName == nameof(Result<Unit>.Success).ToLower())
            {
                reader.Read();
                success = reader.GetBoolean();
            }
            else if (propertyName == nameof(Result<Unit>.Errors).ToLower())
            {
                // Could the JsonConverterForIError be reused here?
                errors = JsonSerializer.Deserialize<List<DeserializedError>>(ref reader, options)!;
            }
        }
        throw new JsonException();

    }

    public override void Write(Utf8JsonWriter writer, Result<Unit> value, JsonSerializerOptions options)
    {
        if (value.Success)
        {
            JsonSerializer.Serialize(writer, new
            {
                value.Success,
            }, options);
        }
        else
        {
            // Could the JsonConverterForIError be reused here?
            JsonSerializer.Serialize(writer, new
            {
                value.Success,
                Errors = value.Errors.Select(x => new { x.Type, x.Message, Data = x.GetData() })
            }, options);
        }

    }
}
