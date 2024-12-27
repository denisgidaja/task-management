using Azure.Messaging.ServiceBus;
using TaskManagement.Contracts;
using TaskManagement.Contracts.ServiceBusInterfaces;
using TaskManagment.Consumer.Notifications;

public class ServiceBusConsumer
{
    private readonly ServiceBusProcessor _updateProcessor;
    private readonly IServiceBusHandler _serviceBusHandler;
    private readonly ILogger<ServiceBusConsumer> _logger;
    private readonly NotificationService _notificationService;


    public ServiceBusConsumer(IServiceBusHandler serviceBusHandler, ILogger<ServiceBusConsumer> logger, NotificationService notificationService)
    {
        _serviceBusHandler = serviceBusHandler;
        _logger = logger;
        _notificationService = notificationService;
    }

    public async Task StartProcessingWithRetryAsync(CancellationToken cancellationToken)
    {
        int retryCount = 0;
        int maxRetries = 5; // need to use configurable parameter

        while (retryCount < maxRetries && !cancellationToken.IsCancellationRequested)
        {
            try
            {
                Console.WriteLine("Attempting to connect to Service Bus...");
                await StartProcessingAsync();
                Console.WriteLine("Connected to Service Bus.");
                return;
            }
            catch (Exception ex)
            {
                retryCount++;
                Console.WriteLine($"Connection failed. Retry {retryCount}/{maxRetries}: {ex.Message}");

                if (retryCount >= maxRetries)
                {
                    Console.WriteLine("Maximum retries reached. Unable to connect to Service Bus.");
                    throw;
                }

                int delay = 2000;
                await Task.Delay(delay, cancellationToken);
            }
        }
    }

    public async Task StartProcessingAsync()
    {
        await _serviceBusHandler.ReceiveMessageAsync(ConsumeFeedbackAddedMessage, OnErrorConsumer, SubscriptionConstants.FeedbackCreateSubscription);
        await _serviceBusHandler.ReceiveMessageAsync(ConsumeFeedbackUpdatedMessage, OnErrorConsumer, SubscriptionConstants.FeedbackUpdateSubscription);
    }

    public void OnErrorConsumer(Exception ex)
    {
        _logger.LogInformation("error with communication with service bus, Error:{Error}", ex.Message);
    }

    public async Task ConsumeFeedbackAddedMessage(Assignment assignment)
    {
        //the task wass successfully added in the database
        _logger.LogInformation("Assingment with id: {TaskId} was successfully created", assignment.ID);

        //send feedback to frontend, using signalr
        await _notificationService.NotifyClients(string.Empty, $"Assingment {assignment.Name} was successfully created");
    }
    public async Task ConsumeFeedbackUpdatedMessage(Assignment assignment)
    {
        //the task wass successfully modified in the database
        _logger.LogInformation("Assingment with id: {TaskId} was successfully updated", assignment.ID);

        //send feedback to frontend, using signalr
        await _notificationService.NotifyClients(string.Empty, $"Assingment {assignment.Name} was successfully updated");
    }
}