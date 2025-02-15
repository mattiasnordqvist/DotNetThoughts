using System.Text.Json;

namespace DotNetThoughts.Results.Json.Tests;

public class Tests
{
    private static JsonSerializerOptions Options => new()
    {
        WriteIndented = true,
        Converters = { new JsonConverterFactoryForResultOfT() }
    };

    [Test]
    public async Task SerializeSuccessfulResultOfUnit()
    {
        var unitResult = UnitResult.Ok;
        var json = JsonSerializer.Serialize(unitResult, Options);
        await Verify(json);
    }

    [Test]
    public async Task SerializeSuccessfulResultOfPrimitive()
    {
        var primitiveResult = Result<int>.Ok(12);
        var json = JsonSerializer.Serialize(primitiveResult, Options);
        await Verify(json);
    }

    public record ComplexType
    {
        public int Id { get; set; } = 123;
        public string Name { get; set; } = "Complex name";

        public List<int> Ints { get; set; } = [1, 2, 3];

        public List<NestedComplexType> ComplexTypes { get; set; } = [new(), new()];

        public record NestedComplexType
        {
            public int Id { get; set; } = 2345;
            public string Description { get; set; } = "Description";
        }
    }
    [Test]
    public async Task SerializeSuccessfulResultOfComplexType()
    {

        var complextResult = Result<ComplexType>.Ok(new ComplexType());
        var json = JsonSerializer.Serialize(complextResult, Options);
        await Verify(json);
    }

    [Test]
    public async Task SerializeErrorResultOfUnit()
    {
        var unitResult = UnitResult.Error(new MyError(123, "hej fel"));
        var json = JsonSerializer.Serialize(unitResult, Options);
        await Verify(json);
    }
    public record MyError(int Code, string Description) : ErrorBase;


    [Test]
    public async Task DeserializeSuccessfulResultOfUnit()
    {
        var unitResultJson = @"{""success"": true}";
        var unitResult = JsonSerializer.Deserialize<Result<Unit>>(unitResultJson, Options);

        await Assert.That(unitResult.Success).IsTrue();
        await Assert.That(unitResult.Value).IsEqualTo(Unit.Instance);
    }

    [Test]
    public async Task DeserializeSuccessfulResultOfPrimitive()
    {
        var unitResultJson = @"{""success"": true, ""value"": 123 }";
        var unitResult = JsonSerializer.Deserialize<Result<int>>(unitResultJson, Options);
        await Assert.That(unitResult.Success).IsTrue();
        await Assert.That(unitResult.Value).IsEqualTo(123);
    }

    [Test]
    public async Task DeserializeSuccessfulResultOfComplextType()
    {
        var complextResult = Result<ComplexType>.Ok(new ComplexType());
        var json = JsonSerializer.Serialize(complextResult, Options);
        var deserialized = JsonSerializer.Deserialize<Result<ComplexType>>(json, Options);
        await Assert.That(deserialized.Success).IsTrue();
        await Assert.That(deserialized.Value).IsEquivalentTo(new ComplexType());
    }

    [Test]
    public async Task DeserializeErrorResultOfUnit()
    {
        var unitResult = UnitResult.Error(new MyError(123, "hej fel"));
        var json = JsonSerializer.Serialize(unitResult, Options);
        var deserialized = JsonSerializer.Deserialize<Result<Unit>>(json, Options);
        await Assert.That(deserialized.Success).IsFalse();
        await Verify(deserialized.Errors);
    }
}