using Microsoft.EntityFrameworkCore;
using Easy_Peasy_ShoppingList.Models;

namespace Easy_Peasy_ShoppingList.Data
{
    public class ShoppingListDbContext : DbContext
    {
        public ShoppingListDbContext(DbContextOptions<ShoppingListDbContext> options)
            : base(options)
        {
            UserCredentials = Set<UserCredentialEntity>();
            FamilyGroups = Set<FamilyGroupEntity>();
        }

        public DbSet<UserCredentialEntity> UserCredentials { get; set; }
        public DbSet<FamilyGroupEntity> FamilyGroups { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserCredentialEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired();
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.Salt).IsRequired();
                entity.Property(e => e.FamilyGroup).IsRequired();
            });

            modelBuilder.Entity<FamilyGroupEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.GroupName).IsRequired();
                entity.Property(e => e.AdminUsername).IsRequired();
            });
        }
    }
}