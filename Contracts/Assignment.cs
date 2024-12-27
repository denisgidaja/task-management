namespace TaskManagement.Contracts
{
    public class Assignment
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public AssignmentStatus Status { get; set; } = AssignmentStatus.NotStarted;
        public string? AssignedTo { get; set; }
    }

    public enum AssignmentStatus
    {
        NotStarted,
        InProgress,
        Completed
    }
}
