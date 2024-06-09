using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace DotNetThoughts.Results.Json;

/// <summary>
/// Creates converters for Result<typeparamref name="T"/> and Results of Unit.
/// Hardcoded to not be case sensitive.
/// Serializes the result as an object with a success-property and either a value-property or an errors-property.
/// Value-property is ommitted if T is Unit, or if success is false.
/// 
/// Feel free to add customization options and figure out how to honor JsonSerializationOptions like PropertyNameCaseSensitive.
/// </summary>
public class JsonConverterFactoryForResultOfT : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
        => typeToConvert.IsGenericType
        && typeToConvert.GetGenericTypeDefinition() == typeof(Result<>);

    public override JsonConverter CreateConverter(
        Type typeToConvert, JsonSerializerOptions options)
    {

        Type elementType = typeToConvert.GetGenericArguments()[0];
        if (typeof(Unit) == elementType)
        {
            var converter = (JsonConverter)Activator.CreateInstance(
                typeof(JsonConverterForResultOfUnit),
                    BindingFlags.Instance | BindingFlags.Public,
                    binder: null,
                    args: null,
                    culture: null)!;
            return converter;
        }
        else
        {
            var converter = (JsonConverter)Activator.CreateInstance(
            typeof(JsonConverterForResultOfT<>).MakeGenericType([elementType]),
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                args: null,
                culture: null)!;
            return converter;
        }
    }
}
