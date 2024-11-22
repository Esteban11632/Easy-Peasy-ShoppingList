namespace TaskManager
{
    public class FileTaskStorage : ITaskStorage // Class for the file task storage
    {
        private readonly string _filePath; // Property for the file path

        public FileTaskStorage(string filePath = "tasks.txt") // Constructor for the file task storage
        {
            _filePath = filePath;
        }

        public Dictionary<string, TodoTask> LoadTasks() // Method to load the tasks
        {
            var tasks = new Dictionary<string, TodoTask>(); // Dictionary for the tasks

            try{
                if (File.Exists(_filePath)) // Checks if the file exists
                {
                    var lines = File.ReadAllLines(_filePath); // Reads all the lines from the file
                    foreach (var line in lines) // Iterates through each line
                    {
                        var parts = line.Split('|'); // Splits the line into parts
                        if (parts.Length >= 4) // Need at least 4 parts (title, description, assignedTo, familyGroup)
                        {
                            string title = parts[0].Trim(); // Title of the task
                            string description = parts[1].Trim(); // Description of the task
                            string assignedTo = string.IsNullOrEmpty(parts[2].Trim()) ? "Unassigned" : parts[2].Trim(); // Assigned to of the task, optional
                            string familyGroup = parts[3].Trim(); // Family group of the task

                            // Validate required fields
                            if (!string.IsNullOrEmpty(title) && 
                                !string.IsNullOrEmpty(description) && 
                                !string.IsNullOrEmpty(familyGroup)) // Checks if the title, description and family group are not empty
                            {
                                var task = new TodoTask(title: title, description: description, assignedTo: assignedTo, familyGroup: familyGroup); // Creates a new task
                                tasks[title] = task; // Adds the task to the dictionary
                            }
                        }
                    }
                }
            }
            catch (Exception ex) // Catches any exceptions
            {
                throw new Exception($"Error loading tasks: {ex.Message}"); // Throws an exception if there is an error loading the tasks
            }
            return tasks; // Returns the tasks
        }

        public void SaveTasks(Dictionary<string, TodoTask> tasks) // Method to save the tasks
        {
            try // Try to save the tasks
            {
                var lines = tasks.Select(t => $"{t.Key}|{t.Value.Description}|{t.Value.AssignedTo}|{t.Value.FamilyGroup}").ToList(); // Selects the title, description, assigned to and family group of the task
                File.WriteAllLines(_filePath, lines); // Writes all the lines to the file
            }
            catch (Exception ex) // Catches any exceptions
            {
                throw new Exception($"Error saving tasks: {ex.Message}"); // Throws an exception if there is an error saving the tasks
            }
        }
    }
} 