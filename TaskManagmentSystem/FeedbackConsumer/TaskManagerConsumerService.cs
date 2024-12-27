using TaskManagement.Contracts.ConsumerInterfaces;
using TaskManagement.Contracts;
using TaskManagement.Contracts.RepositoryInterfaces;
using TaskManagement.Contracts.ServiceBusInterfaces;

namespace TaskManagement.FeedbackConsumer
{
    public class TaskManagerConsumerService : ITaskManagerConsumerService
    {
        private readonly IRepositoryManager _repoManager;
        private readonly IServiceBusHandler _serviceBusHandler;
        private readonly ILogger<TaskManagerConsumerService> _logger;
        public TaskManagerConsumerService(IServiceBusHandler serviceBusHandler, IRepositoryManager repoManager, ILogger<TaskManagerConsumerService> logger)
        {
            _repoManager = repoManager;
            _serviceBusHandler = serviceBusHandler;
            _logger = logger;


        }
        public async Task RegisterConsumer()
        {
            await _serviceBusHandler.ReceiveMessageAsync(ConsumeFeedbackAddedMessage, OnErrorConsumer, SubscriptionConstants.FeedbackCreateSubscription);
            await _serviceBusHandler.ReceiveMessageAsync(ConsumeFeedbackUpdatedMessage, OnErrorConsumer, SubscriptionConstants.FeedbackUpdateSubscription);
        }
        public async Task ConsumeFeedbackAddedMessage(Assignment assignment)
        {
            //the task wass successfully added in the database
            _logger.LogInformation("Assingment with id: {TaskId} was successfully added", assignment.ID);

            //send feedback to frontend, mabye using signalr
        }
        public async Task ConsumeFeedbackUpdatedMessage(Assignment assignment)
        {
            //the task wass successfully modified in the database
            _logger.LogInformation("Assingment with id: {TaskId} was successfully updated", assignment.ID);

            //send feedback to frontend, mabye using signalr
        }

        public void OnErrorConsumer(Exception ex)
        {
            _logger.LogInformation("error with communication with service bus, Error:{Error}", ex.Message);
        }
    }
}
