using System.ComponentModel.DataAnnotations;

namespace Easy_Peasy_ShoppingList.Models
{
    public class TodoTaskEntity
    {
        [Key]
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string AssignedTo { get; set; }
        public required string FamilyGroup { get; set; }
        public bool IsDone { get; set; } = false;
    }
}
