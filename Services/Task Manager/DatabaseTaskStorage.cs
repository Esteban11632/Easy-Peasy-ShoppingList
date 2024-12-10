using Easy_Peasy_ShoppingList.Data;
using Easy_Peasy_ShoppingList.Models;
using Microsoft.EntityFrameworkCore;

namespace TaskManager
{
    public class DatabaseTaskStorage : ITaskStorage
    {
        private readonly ShoppingListDbContext _context;

        public DatabaseTaskStorage(ShoppingListDbContext context)
        {
            _context = context;
        }

        public Dictionary<string, TodoTask> LoadTasks()
        {
            var taskEntities = _context.Tasks.ToList();
            return taskEntities.ToDictionary(
                t => t.Title,
                t => new TodoTask(
                    t.Title,
                    t.Description,
                    t.AssignedTo,
                    t.FamilyGroup,
                    t.IsDone
                )
            );
        }

        public List<TodoTask> GetTasksByFamilyGroup(string familyGroup)
        {
            return _context.Tasks
                .Where(t => t.FamilyGroup.Equals(familyGroup, StringComparison.OrdinalIgnoreCase))
                .Select(t => new TodoTask(
                    t.Title,
                    t.Description,
                    t.AssignedTo,
                    t.FamilyGroup,
                    t.IsDone
                ))
                .ToList();
        }

        public void SaveTasks(Dictionary<string, TodoTask> tasks)
        {
            // Start a transaction
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                // Clear existing tasks
                _context.Tasks.RemoveRange(_context.Tasks);

                // Add new tasks
                foreach (var task in tasks.Values)
                {
                    _context.Tasks.Add(new TodoTaskEntity
                    {
                        Title = task.Title,
                        Description = task.Description,
                        AssignedTo = task.AssignedTo,
                        FamilyGroup = task.FamilyGroup
                    });
                }

                _context.SaveChanges();
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public void UpdateTaskStatus(string title, string familyGroup, bool isDone)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var task = _context.Tasks
                    .FirstOrDefault(t => t.Title == title && 
                                       t.FamilyGroup.Equals(familyGroup, StringComparison.OrdinalIgnoreCase));

                if (task != null)
                {
                    if (isDone)
                    {
                        // Remove the task if it's marked as done
                        _context.Tasks.Remove(task);
                    }
                    else
                    {
                        // Update the status if not done
                        task.IsDone = isDone;
                    }
                    
                    _context.SaveChanges();
                    transaction.Commit();
                }
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}