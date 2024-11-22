namespace TaskManager
{
    public interface ICreateTask
    {
        void CreateTask(string title, string description, string assignedTo, string familyGroup); // Method to create a task
    }
}