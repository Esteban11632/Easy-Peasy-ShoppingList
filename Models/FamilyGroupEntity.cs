namespace Easy_Peasy_ShoppingList.Models
{
    public class FamilyGroupEntity
    {
        public int Id { get; set; }
        public required string GroupName { get; set; }
        public required string AdminUsername { get; set; }
    }
}