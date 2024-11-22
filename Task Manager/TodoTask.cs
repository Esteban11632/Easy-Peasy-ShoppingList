namespace TaskManager
{
    public class TodoTask // Class for the task
    {
        public string Title { get; } // Property for the title of the task
        public string Description { get; } // Property for the description of the task
        public string AssignedTo { get; private set; } // Property for the assigned to of the task
        public string FamilyGroup { get; } // Property for the family group of the task

        public TodoTask(string title, string description, string assignedTo = "Unassigned", string familyGroup = "Unassigned") // Constructor for the task
        {
            Title = title ?? throw new ArgumentNullException(nameof(title)); // Title of the task
            Description = description ?? throw new ArgumentNullException(nameof(description)); // Description of the task
            AssignedTo = assignedTo ?? "Unassigned"; // Assigned to of the task
            FamilyGroup = familyGroup ?? throw new ArgumentNullException(nameof(familyGroup)); // Family group of the task
        }

        public void AssignTo(string user) // Method to assign the task to a user
        {
            AssignedTo = user ?? throw new ArgumentNullException(nameof(user)); // Assigned to of the task
        }
    }
} 