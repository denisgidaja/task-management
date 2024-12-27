using TaskManagement.Contracts.RepositoryInterfaces;

namespace TaskManagement.Infrastructure
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly RepositoryContext _repositoryContext;
        private readonly Lazy<IAssignmentRepository> _assignmentRepo;

        private readonly Dictionary<Type, object> _repositoryList;

        public RepositoryManager(RepositoryContext repositoryContext)
        {
            _repositoryContext = repositoryContext;
            _assignmentRepo = new Lazy<IAssignmentRepository>(() => new AssignmentRepository(repositoryContext));
        }        

        public IAssignmentRepository AssignmentRepo => _assignmentRepo.Value;
    }
}
