namespace Easy_Peasy_ShoppingList.Models
{
    public class UserCredentialEntity
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string PasswordHash { get; set; }
        public required string Salt { get; set; }
        public bool IsAdmin { get; set; }
        public required string FamilyGroup { get; set; }
    }
}