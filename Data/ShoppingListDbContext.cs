using Microsoft.EntityFrameworkCore;
using Easy_Peasy_ShoppingList.Models;

namespace Easy_Peasy_ShoppingList.Data
{
    public class ShoppingListDbContext : DbContext
    {
        public DbSet<UserCredentialEntity> UserCredentials => Set<UserCredentialEntity>();
        public DbSet<FamilyGroupEntity> FamilyGroups => Set<FamilyGroupEntity>();

        public DbSet<ShoppingItem> ShoppingItems { get; set; } = null!;
        public DbSet<WishlistItem> WishlistItems { get; set; } = null!;
        public DbSet<TodoTaskEntity> Tasks { get; set; } = null!;

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

            modelBuilder.Entity<ShoppingItem>()
                .HasIndex(s => s.FamilyGroup);

            modelBuilder.Entity<WishlistItem>()
                .HasIndex(w => w.FamilyGroup);

            modelBuilder.Entity<TodoTaskEntity>()
                .HasIndex(t => new { t.Title, t.FamilyGroup })
                .IsUnique();
        }
    }
}