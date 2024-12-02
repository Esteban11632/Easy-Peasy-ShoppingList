using Microsoft.EntityFrameworkCore;
using Easy_Peasy_ShoppingList.Models;

namespace Easy_Peasy_ShoppingList.Data
{
    public class ShoppingListDbContext : DbContext
    {
        public DbSet<UserCredentialEntity> UserCredentials => Set<UserCredentialEntity>();
        public DbSet<FamilyGroupEntity> FamilyGroups => Set<FamilyGroupEntity>();

        public ShoppingListDbContext(DbContextOptions<ShoppingListDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FamilyGroupEntity>()
                .HasIndex(f => f.GroupName)
                .IsUnique();

            modelBuilder.Entity<UserCredentialEntity>()
                .HasIndex(u => u.Username)
                .IsUnique();
        }
    }
}