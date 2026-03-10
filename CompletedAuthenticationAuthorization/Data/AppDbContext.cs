using Microsoft.EntityFrameworkCore;
using AuthenticationAuthorization.Models;
namespace AuthenticationAuthorization.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
     public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>(); 

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(u=>u.Email).IsUnique();
        modelBuilder.Entity<User>().HasIndex(u=>u.Username).IsUnique();

        modelBuilder.Entity<RefreshToken>().HasOne(r=> r.User).WithMany(u=>u.RefreshTokens).HasForeignKey(r=>r.UserId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PasswordResetToken>()
            .HasOne(r => r.User)
            .WithMany(u => u.PasswordResetTokens)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Seed an admin user
        modelBuilder.Entity<User>().HasData(new User
        {
            Id = 1,
            Username = "admin",
            Email = "admin@app.com",
            PasswordHash = "$2a$12$Coh2usqFXkDAI3uF4izz2.RlK8mozAjKS9SgfnSDBTUgYQF03u8M2", //admin123
            Role = "Admin",
            IsActive = true,
            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });
    }
}

