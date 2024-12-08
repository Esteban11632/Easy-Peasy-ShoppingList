using System.ComponentModel.DataAnnotations;

namespace Easy_Peasy_ShoppingList.Models
{
    public class ShoppingItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
        public decimal Price { get; set; } = 0;

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; } = 1;

        [Required]
        public string FamilyGroup { get; set; } = string.Empty;
    }
}
