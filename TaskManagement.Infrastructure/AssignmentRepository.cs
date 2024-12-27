using TaskManagement.Contracts;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Contracts.RepositoryInterfaces;

namespace TaskManagement.Infrastructure
{
    public class AssignmentRepository : IAssignmentRepository
    {
        private RepositoryContext RepositoryContext;
        public AssignmentRepository(RepositoryContext repositoryContext)
        {
            RepositoryContext = repositoryContext;
        }
        public async Task Add(Assignment assignment)
        {
            RepositoryContext.Set<Assignment>().Add(assignment);
            await RepositoryContext.SaveChangesAsync();
        }

        public async Task UpdateStatus(int assignmentId, AssignmentStatus status)
        {
            var task = await RepositoryContext.Assignments.FirstOrDefaultAsync(u => u.ID == assignmentId);

            if (task == null)
            {
                throw new Exception("Task not found");
            }
            task.Status = status;
            await RepositoryContext.SaveChangesAsync();
        }
        public async Task<List<Assignment>> GetAllAsync()
        {
            return await RepositoryContext.Assignments.ToListAsync();
        }
        public async Task<List<Assignment>> GetAllAsync(AssignmentStatus status)
        {
            return await RepositoryContext.Assignments.Where(a => a.Status == status).ToListAsync();
        }
    }
}
