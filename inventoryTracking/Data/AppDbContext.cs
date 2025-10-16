using EnvanterBackend.Entities;
using EnvanterBackend.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace EnvanterBackend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSet’ler
        public DbSet<User> Users { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<InventoryHistory> InventoryHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Admin seed
            var admin = new User
            {
                Id = 1,
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                Role = UserRole.Admin,
                CanInventory = true,
                CanLogs = true,
                CanUsers = true,
                CreatedAt = DateTime.UtcNow
            };

            modelBuilder.Entity<User>().HasData(admin);

            // User → Logs ilişkisi
            modelBuilder.Entity<User>()
                .HasMany(u => u.Logs)
                .WithOne(l => l.User)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            // User.Role string enum conversion
            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            // InventoryHistory ilişkisi
            modelBuilder.Entity<InventoryHistory>()
                .HasOne(h => h.Inventory)
                .WithMany()
                .HasForeignKey(h => h.InventoryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
