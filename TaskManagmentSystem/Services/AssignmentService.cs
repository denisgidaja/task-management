using Microsoft.AspNetCore.Mvc;
using TaskManagement.Contracts;
using TaskManagement.Contracts.RepositoryInterfaces;
using TaskManagement.Contracts.ServiceBusInterfaces;
using TaskManagement.Contracts.Services;
using TaskManagement.ServiceBus;

namespace TaskManagment.Services
{
    public class AssignmentService : IAssignmentService
    {
        private readonly IServiceBusHandler _serviceBus;
        private readonly ILogger _logger;
        private readonly IRepositoryManager _repoManager;
        public AssignmentService(IServiceBusHandler serviceBusHandler, ILogger logger, IRepositoryManager repoManager)
        {
            _serviceBus = serviceBusHandler;
            _logger = logger;
            _repoManager = repoManager;
        }
        public async Task UpdateTaskStatus(Assignment task)
        {
            _logger.LogInformation("Init method Update Task Status for Task: {TaskId}", task.ID);
            await _serviceBus.SendUpdateMessageAsync(task);
        }
        public async Task AddTask(Assignment task)
        {
            _logger.LogInformation("Init method Add new Task");
            await _serviceBus.SendCreateMessageAsync(task);
        }
        public async Task<List<Assignment>> GetTasks(AssignmentStatus status)
        {
            _logger.LogInformation("Init method Get All Tasks for status: {Status}", status.ToString());
            return await _repoManager.AssignmentRepo.GetAllAsync(status);
        }
        public async Task<List<Assignment>> GetTasks()
        {
            _logger.LogInformation("Init method Get All Tasks");
            return await _repoManager.AssignmentRepo.GetAllAsync();
        }
    }
}
