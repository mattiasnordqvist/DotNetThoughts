namespace DotNetThoughts.Results.Json;

public class DeserializedError : IError
{
    public required string Type { get; init; }
    public required string Message { get; init; }
    public required Dictionary<string, object?> Data { get; init; } = [];
}