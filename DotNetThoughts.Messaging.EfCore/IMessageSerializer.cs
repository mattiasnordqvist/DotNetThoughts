namespace DotNetThoughts.Messaging.EfCore;

public interface IMessageSerializer
{
    string Serialize(Event @event);
}
