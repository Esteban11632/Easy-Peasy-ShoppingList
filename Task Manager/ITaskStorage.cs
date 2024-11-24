namespace TaskManager
{
    public interface ITaskStorage // Interface for the task storage
    {
        Dictionary<string, TodoTask> LoadTasks(); // Method to load the tasks
        List<TodoTask> GetTasksByFamilyGroup(string familyGroup); // Method to get the tasks by family group
        void SaveTasks(Dictionary<string, TodoTask> tasks); // Method to save the tasks
    }
} 