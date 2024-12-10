namespace TaskManager
{
    public class TodoTask
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string AssignedTo { get; set; }
        public string FamilyGroup { get; set; }
        public bool IsDone { get; set; }

        public TodoTask(string title, string description, string assignedTo = "Unassigned", string familyGroup = "Unassigned", bool isDone = false)
        {
            Title = title ?? throw new ArgumentNullException(nameof(title)); // Title of the task
            Description = description ?? throw new ArgumentNullException(nameof(description)); // Description of the task
            AssignedTo = assignedTo ?? "Unassigned"; // Assigned to of the task
            FamilyGroup = familyGroup ?? "Unassigned"; // Family group of the task
            IsDone = isDone;
        }

        public void AssignTo(string user) // Method to assign the task to a user
        {
            AssignedTo = user ?? throw new ArgumentNullException(nameof(user)); // Assigned to of the task
        }
    }
}