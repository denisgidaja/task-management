using Azure.Messaging.ServiceBus;
using System.Text.Json;
using TaskManagement.Contracts;
using TaskManagement.Contracts.ServiceBusInterfaces;

namespace TaskManagement.ServiceBus
{
    public class ServiceBusHandler : IServiceBusHandler
    {
        private readonly ServiceBusClient _client;
        private readonly ServiceBusSender _sender;
        private readonly string _topicName;
        public static bool ConsumerConnected = false;

        //needs to be a configurable parameter
        private const int MaxRetryAttempts = 5;
        public ServiceBusHandler(string connectionString, string topicName)
        {
            _client = new ServiceBusClient(connectionString);
            _topicName = topicName;
            _sender = _client.CreateSender(_topicName);
        }
        public async Task SendMessageAsync(object message, string operationType)
        {
            int retryCount = 0;
            while (retryCount < MaxRetryAttempts)
            {
                try
                {
                    var jsonMessage = JsonSerializer.Serialize(message);
                    var serviceBusMessage = new ServiceBusMessage(jsonMessage)
                    {
                        ApplicationProperties = { { "OperationType", operationType } }
                    };
                    await _sender.SendMessageAsync(serviceBusMessage);

                    Console.WriteLine("Message sent successfully.");
                    return;
                }
                catch (ServiceBusException ex) when (ex.IsTransient)
                {
                    retryCount++;
                    Console.WriteLine($"Transient error occurred. Retry attempt {retryCount}.");

                    if (retryCount >= MaxRetryAttempts)
                    {
                        Console.WriteLine($"Failed to send message after {retryCount} retries: {ex.Message}");
                        throw;
                    }

                    int delay = 1000;
                    Console.WriteLine($"Waiting {delay}ms before next retry...");
                    await Task.Delay(delay);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Non-transient error occurred: {ex.Message}");
                    throw;
                }
            }
        }
        public async Task SendCreateMessageAsync(object message)
        {
            await SendMessageAsync(message, OperationConstants.Create);
        }

        public async Task SendUpdateMessageAsync(object message)
        {
            await SendMessageAsync(message, OperationConstants.Update);
        }

        public async Task ReceiveMessageAsync(Func<Assignment, Task> onProcessMessage, Action<Exception> onError, string subscription)
        {
            var processor = _client.CreateProcessor(_topicName, subscription);
            ConsumerConnected = true;

            processor.ProcessMessageAsync += async args =>
            {
                var jsonMessage = args.Message.Body.ToString();
                var task = JsonSerializer.Deserialize<Assignment>(jsonMessage);

                // execute custom action
                await onProcessMessage(task);
                await args.CompleteMessageAsync(args.Message);
            };
            processor.ProcessErrorAsync += (args) => {
                onError(args.Exception);
                ConsumerConnected = false;
                return Task.CompletedTask;
            };

            await processor.StartProcessingAsync();
        }
    }
}
