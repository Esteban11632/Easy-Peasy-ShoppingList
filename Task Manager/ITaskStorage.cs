namespace TaskManager
{
    public interface ITaskStorage // Interface for the task storage
    {
        Dictionary<string, TodoTask> LoadTasks(); // Method to load the tasks
        void SaveTasks(Dictionary<string, TodoTask> tasks); // Method to save the tasks
    }
} 