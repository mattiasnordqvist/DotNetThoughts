namespace DotNetThoughts.Messaging.EfCore;

public interface IMessageRelayServiceNotifier
{
    Task Notify();
}
