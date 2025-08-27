using System.Text.Json;

namespace DotNetThoughts.Results.Json;

public static class JsonSerializerOptionsExtensions
{
    /// <summary>
    /// Adds support for serializing and deserializing Result of T and Unit.
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public static JsonSerializerOptions AddResultConverters(this JsonSerializerOptions options)
    {
        options.Converters.Add(new JsonConverterFactoryForResultOfT());
        options.Converters.Add(new JsonConverterForIError());
        return options;
    }
}
