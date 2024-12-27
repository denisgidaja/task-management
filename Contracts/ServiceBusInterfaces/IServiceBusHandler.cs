
namespace TaskManagement.Contracts.ServiceBusInterfaces
{
    public interface IServiceBusHandler
    {
        Task SendCreateMessageAsync(object message);
        Task SendMessageAsync(object message, string subscription);
        Task SendUpdateMessageAsync(object message);
        Task ReceiveMessageAsync(Func<Assignment, Task> processMessage, Action<Exception> onError, string subscription);
    }
}
