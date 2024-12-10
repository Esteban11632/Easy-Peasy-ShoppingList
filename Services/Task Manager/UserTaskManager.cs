namespace TaskManager
{
    public class UserTaskManager : IViewTask, IUpdateTask
    {
        private readonly ITaskStorage _storage; // The storage for the tasks
        public UserTaskManager(ITaskStorage storage) // Constructor for the UserTaskManager class
        {
            _storage = storage; // Initializes the storage
        }
        public void DisplayTask(string username, string familyGroup) // Method to display the task
        {
            var tasks = _storage.GetTasksByFamilyGroup(familyGroup); // Gets the tasks by family group
            var userTasks = tasks.Where(task => task.AssignedTo == username).ToList(); // Gets the tasks by username

            if (!userTasks.Any()) // Checks if the user tasks are empty
            {
                Console.WriteLine("No tasks found for the user."); // Displays a message if the user tasks are empty
                return;
            }
            Console.WriteLine($"Tasks for the {username} in family {familyGroup}:"); // Displays a message if the user tasks are not empty
            Console.WriteLine("--------------------------------");

            foreach (var task in userTasks) // Iterates through the user tasks
            {
                Console.WriteLine($"Title: {task.Title}"); // Displays the title of the task
                Console.WriteLine($"Description: {task.Description}"); // Displays the description of the task
                Console.WriteLine("------------------------");
            }
        }

        public void UpdateTaskAsync(TodoTask task) // Method to update the task to done or not done
        {
            _storage.UpdateTaskStatus(task.Title, task.FamilyGroup, task.IsDone); // Updates the task to done or not done
        }

    }
}