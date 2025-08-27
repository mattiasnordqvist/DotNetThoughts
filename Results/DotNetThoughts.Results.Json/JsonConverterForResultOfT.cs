using System.Text.Json.Serialization;
using System.Text.Json;

namespace DotNetThoughts.Results.Json;

internal class JsonConverterForResultOfT<T> : JsonConverter<Result<T>>
{
    public override Result<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        T? t = default;
        bool success = false;
        List<DeserializedError> errors = [];
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return success
                    ? Result<T>.Ok(t!)
                    : Result<T>.Error(errors);
            }
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }
            string propertyName = reader.GetString()!.ToLower();
            if (propertyName == nameof(Result<T>.Success).ToLower())
            {
                reader.Read();
                success = reader.GetBoolean();
            }
            else if (propertyName == nameof(Result<T>.Value).ToLower())
            {
                t = JsonSerializer.Deserialize<T>(ref reader, options);
            }
            else if (propertyName == nameof(Result<T>.Errors).ToLower())
            {
                // Could the JsonConverterForIError be reused here?
                errors = JsonSerializer.Deserialize<List<DeserializedError>>(ref reader, options)!;
            }
        }
        throw new JsonException();

    }

    public override void Write(Utf8JsonWriter writer, Result<T> value, JsonSerializerOptions options)
    {
        if (value.Success)
        {
            JsonSerializer.Serialize(writer, new
            {
                value.Success,
                value.Value
            }, options);
        }
        else
        {
            // Could the JsonConverterForIError be reused here?
            JsonSerializer.Serialize(writer, new
            {
                value.Success,
                Errors = value.Errors.Select(x => new { x.Type, x.Message, x.Data })
            }, options);
        }
    }
}
