using System.ComponentModel.DataAnnotations;

namespace Easy_Peasy_ShoppingList.Models
{
    public class WishlistItem
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Item name is required.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required.")]
        public string Category { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
        public decimal Price { get; set; } = 0;

        [Required]
        public string RequestingUser { get; set; } = string.Empty;

        [Required]
        public string FamilyGroup { get; set; } = string.Empty;
    }
}
