namespace DotNetThoughts.Messaging.EfCore;

public class OutboxEntry
{
    public int? Id { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public required string Data { get; set; }
    public required string EventTypeName { get; set; }
}