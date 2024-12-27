
namespace TaskManagement.Contracts.RepositoryInterfaces
{
    public interface IAssignmentRepository
    {
        Task Add(Assignment assignment);
        Task UpdateStatus(int assignmentId, AssignmentStatus status);
        Task<List<Assignment>> GetAllAsync();
        Task<List<Assignment>> GetAllAsync(AssignmentStatus status);
    }
}
