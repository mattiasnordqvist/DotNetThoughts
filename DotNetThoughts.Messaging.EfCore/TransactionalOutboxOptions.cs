namespace DotNetThoughts.Messaging.EfCore;

public class TransactionalOutboxOptions
{
    public string Table { get; set; } = "TransactionalOutbox";
    public string Schema { get; set; } = "dbo";
    public Func<DateTimeOffset> Now { get; set; } = () => DateTimeOffset.UtcNow;
}