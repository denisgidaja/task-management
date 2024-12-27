using TaskManagement.Contracts;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Contracts.ServiceBusInterfaces;
using TaskManagement.Contracts.RepositoryInterfaces;
using TaskManagement.Contracts.Services;

namespace TaskManagement.Controllers
{
    public class AssignmentController : ControllerBase
    {
        private readonly IServiceBusHandler _serviceBus;
        private readonly IServiceManager _serviceManager;
        private readonly ILogger<AssignmentController> _logger;

        public AssignmentController(IServiceBusHandler serviceBus, IServiceManager serviceManager, ILogger<AssignmentController> logger)
        {
            _logger = logger;
            _serviceBus = serviceBus;
            _serviceManager = serviceManager;
        }

        [HttpGet("api/[controller]/All")]
        public async Task<List<Assignment>> GetTasks()
        {
            return await _serviceManager.AssignmentService.GetTasks();
        }
        [HttpGet("api/[controller]/All/{status}")]
        public async Task<List<Assignment>> GetTasks(AssignmentStatus status)
        {
            return await _serviceManager.AssignmentService.GetTasks(status);
        }

        [HttpPost("[controller]/add")]
        public async Task<IActionResult> AddTask([FromBody] Assignment task)
        {
            await _serviceManager.AssignmentService.AddTask(task);

            return Ok("The assignment is being crated. You will get notified when the process finishes");
        }

        [HttpPut("[controller]/update")]
        public async Task<IActionResult> UpdateTaskStatus([FromBody] Assignment task)
        {
            await _serviceManager.AssignmentService.UpdateTaskStatus(task);

            return Ok("The update process started. You will get notified when the process finishes");
        }
    }
}
