namespace TaskManager
{
    public interface IUpdateTask
    {
        void UpdateTaskAsync(TodoTask task); // Method to update the task to done or not done
    }
}