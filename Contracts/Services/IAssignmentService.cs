
namespace TaskManagement.Contracts.Services
{
    public interface IAssignmentService
    {
        Task UpdateTaskStatus(Assignment task);
        Task AddTask(Assignment task);
        Task<List<Assignment>> GetTasks(AssignmentStatus status);
        Task<List<Assignment>> GetTasks();
    }
}
