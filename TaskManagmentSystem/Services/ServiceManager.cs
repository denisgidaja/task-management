using TaskManagement.Contracts.RepositoryInterfaces;
using TaskManagement.Contracts.ServiceBusInterfaces;
using TaskManagement.Contracts.Services;
using TaskManagement.ServiceBus;

namespace TaskManagment.Services
{
    public class ServiceManager : IServiceManager
    {

        private readonly Lazy<IAssignmentService> _assignmentService;

        public ServiceManager(IRepositoryManager repositoryManager, IServiceBusHandler serviceBus, IRepositoryManager repoManager, ILogger<ServiceManager> logger)
        {
            _assignmentService = new Lazy<IAssignmentService>(() => new AssignmentService(serviceBus, logger, repositoryManager));
        }

        public IAssignmentService AssignmentService => _assignmentService.Value;
    }
}
