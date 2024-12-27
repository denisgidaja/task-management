using TaskManagement.Contracts.ConsumerInterfaces;
using TaskManagement.Contracts;
using TaskManagement.Contracts.RepositoryInterfaces;
using TaskManagement.Contracts.ServiceBusInterfaces;

namespace TaskManagement.Consumer
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
            await _serviceBusHandler.ReceiveMessageAsync(ConsumeAssignmentAddedMessage, OnErrorConsumer, SubscriptionConstants.CreateSubscription);
            await _serviceBusHandler.ReceiveMessageAsync(ConsumeAssignmentUpdatedMessage, OnErrorConsumer, SubscriptionConstants.UpdateSubscription);
            //if it is needed to registed another consumer it is enough to add here
        }
        public async Task ConsumeAssignmentAddedMessage(Assignment assignment)
        {
            //add the assignment in the database
            await _repoManager.AssignmentRepo.Add(assignment);

            //send feedback to web api project
            await _serviceBusHandler.SendMessageAsync(assignment, OperationConstants.FeedbackCreated);
        }
        public async Task ConsumeAssignmentUpdatedMessage(Assignment assignment)
        {
            await _repoManager.AssignmentRepo.UpdateStatus(assignment.ID, assignment.Status);

            //send feedback to web api project
            await _serviceBusHandler.SendMessageAsync(assignment, OperationConstants.FeedbackUpdated);
        }

        public void OnErrorConsumer(Exception ex)
        {
            _logger.LogInformation("error with communication with service bus, Error:{Error}", ex.Message);
        }
    }
}
