namespace TaskManager
{
    public class TodoTask
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string AssignedTo { get; set; }
        public string FamilyGroup { get; set; }
        public bool IsDone { get; set; }

        public TodoTask(string title, string description, string assignedTo, string familyGroup, bool isDone = false)
        {
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            AssignedTo = assignedTo ?? throw new ArgumentNullException(nameof(assignedTo));
            FamilyGroup = familyGroup ?? throw new ArgumentNullException(nameof(familyGroup));
            IsDone = isDone;
        }

        public TodoTask()
        {
            Title = string.Empty;
            Description = string.Empty;
            AssignedTo = string.Empty;
            FamilyGroup = string.Empty;
        }
    }
}