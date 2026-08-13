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

        await context.Database.MigrateAsync();

        if (!await context.AdminUsers.AnyAsync())
        {
            var passwordHasher = new PasswordHasher<User>();

            var defaultAdmin = new User
            {
                Id = Guid.NewGuid(),
                Username = "admin",
                CreatedAt = DateTime.UtcNow
            };

            defaultAdmin.PasswordHash = passwordHasher.HashPassword(defaultAdmin, "AdminPassword123!");

            await context.AdminUsers.AddAsync(defaultAdmin);
            await context.SaveChangesAsync();
        }
    }

    public static async Task SeedClientAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await context.Database.MigrateAsync();

        if (!await context.Clients.AnyAsync())
        {

            var testClient = new Client
            {
                Id = Guid.NewGuid(),
                FullName = "test user 1",
                Email = "test@user.com",
                Phone = "+51999888777",
                CreatedAt = DateTime.UtcNow
            };


            await context.Clients.AddAsync(testClient);
            await context.SaveChangesAsync();
        }
    }
}