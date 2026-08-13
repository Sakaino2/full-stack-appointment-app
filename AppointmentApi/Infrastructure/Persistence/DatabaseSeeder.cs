using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static async Task SeedAdminUserAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Ensure database is created and pending migrations are applied
        await context.Database.MigrateAsync();

        // Check if admin user already exists
        if (!await context.AdminUsers.AnyAsync())
        {
            var passwordHasher = new PasswordHasher<AdminUser>();

            var defaultAdmin = new AdminUser
            {
                Id = Guid.NewGuid(),
                Username = "admin",
                CreatedAt = DateTime.UtcNow
            };

            // Securely hash initial password "AdminPassword123!"
            defaultAdmin.PasswordHash = passwordHasher.HashPassword(defaultAdmin, "AdminPassword123!");

            await context.AdminUsers.AddAsync(defaultAdmin);
            await context.SaveChangesAsync();
        }
    }
}