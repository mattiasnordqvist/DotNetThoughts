using System.Text.Json;
using System.Text.Json.Nodes;

namespace DotNetThoughts.Json;

public class JsonNormalizer
{
    public class Options
    {
        private bool _writeIndented = true;
        public bool WriteIndented
        {
            get => _writeIndented;
            set
            {
                _writeIndented = value;
                _cachedJsonSerializerOptions = null;
            }
        }

        private int _indentSize = 4;
        public int IndentSize
        {
            get => _indentSize;
            set
            {
                _indentSize = value;
                _cachedJsonSerializerOptions = null;
            }
        }

        /// <summary>
        /// If true, the properties of JSON objects will be sorted alphabetically.
        /// </summary>
        public bool OrderProperties { get; set; } = false;

        private JsonSerializerOptions? _cachedJsonSerializerOptions;

        internal JsonSerializerOptions GetJsonSerializerOptions()
        {
            if (_cachedJsonSerializerOptions != null)
            {
                return _cachedJsonSerializerOptions;
            }
            _cachedJsonSerializerOptions = new JsonSerializerOptions
            {
                WriteIndented = WriteIndented,
                IndentSize = IndentSize,
            };
            return _cachedJsonSerializerOptions;
        }
    }

    public static string Normalize(string json, Options options)
    {
        var parsedJson = JsonNode.Parse(json);
        if (options.OrderProperties)
        {
            // Sort the properties of the JSON object recursively
            SortJsonProperties(parsedJson);

        }
        return JsonSerializer.Serialize(parsedJson, options.GetJsonSerializerOptions());
    }

    /// <summary>
    /// Performs a recursive in-place sort of the properties of a JSON object.
    /// </summary>
    /// <param name="parsedJson"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    private static void SortJsonProperties(JsonNode? parsedJson)
    {
        if (parsedJson is JsonObject jsonObject)
        {
            var sortedProperties = jsonObject.OrderBy(kvp => kvp.Key).ToArray();
            jsonObject.Clear();
            foreach (var kvp in sortedProperties)
            {
                jsonObject.Add(kvp.Key, kvp.Value);
                SortJsonProperties(kvp.Value);
            }
        }
        else if (parsedJson is JsonArray jsonArray)
        {
            foreach (var item in jsonArray)
            {
                SortJsonProperties(item);
            }
        }
    }
}
