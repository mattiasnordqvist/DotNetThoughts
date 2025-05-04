namespace DotNetThoughts.Json.Tests;

public class JsonNormalizerTests
{
    [Test]
    public async Task TestJsonNormalization()
    {
        var json = "{ \"name\": \"John\", \"age\": 30, \"city\": \"New York\" }";
        var expected = """
            {
              "name": "John",
              "age": 30,
              "city": "New York"
            }
            """;

        var options = new JsonNormalizer.Options
        {
            WriteIndented = true,
            IndentSize = 2,
            OrderProperties = false,
        };
        var normalizedJson = JsonNormalizer.Normalize(json, options);

        await Assert.That(normalizedJson).IsEqualTo(expected);
    }

    [Test]
    public async Task TestJsonOrderNormalization()
    {
        var json = "{ \"name\": \"John\", \"age\": 30, \"city\": \"New York\" }";
        var expected = "{\"age\":30,\"city\":\"New York\",\"name\":\"John\"}";

        var options = new JsonNormalizer.Options
        {
            WriteIndented = false,
            OrderProperties = true,
        };
        var normalizedJson = JsonNormalizer.Normalize(json, options);

        await Assert.That(normalizedJson).IsEqualTo(expected);
    }
}