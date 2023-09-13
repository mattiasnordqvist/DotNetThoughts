using System.Reflection;

namespace DotNetThoughts.Messaging;

public static class EventExtensions
{
    public static string GetEventName(this Event @event) => @event.GetType().GetEventName();
    public static string GetEventName(this Type eventType)
    {
        if (!eventType.IsAssignableTo(typeof(Event)))
        {
            throw new Exception($"Type {eventType.FullName} is not an Event");
        }

        return eventType.GetCustomAttribute<EventNameAttribute>()?.Name ?? (eventType.Name.EndsWith("Event") ? eventType.Name.Replace("Event", "") : throw new Exception("Cant figure out event name"));
    }
}

