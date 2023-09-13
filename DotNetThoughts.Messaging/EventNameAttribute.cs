namespace DotNetThoughts.Messaging;

[AttributeUsage(AttributeTargets.Class)]
public class EventNameAttribute : Attribute
{
    public EventNameAttribute(string name)
    {
        Name = name;
    }

    public string Name { get; }
}

