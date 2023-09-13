namespace DotNetThoughts.Messaging;

public interface IEventSource
{
    IEnumerable<Event> Events { get; }
    void ClearEvents();
}