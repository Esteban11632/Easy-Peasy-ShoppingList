using Microsoft.EntityFrameworkCore;
using Easy_Peasy_ShoppingList.Models;

namespace Easy_Peasy_ShoppingList.Data
{
    public class ShoppingListDbContext : DbContext
    {
        public ShoppingListDbContext(DbContextOptions<ShoppingListDbContext> options)
            : base(options)
        {
        }

        public required DbSet<UserCredentialEntity> UserCredentials { get; set; }
        public required DbSet<FamilyGroupEntity> FamilyGroups { get; set; }
    }
}