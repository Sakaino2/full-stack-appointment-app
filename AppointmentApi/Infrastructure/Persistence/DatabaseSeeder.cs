using Application.Dtos;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static async Task SeedAdminUserAsync(IServiceProvider services)
    {
        var userService = services.GetRequiredService<IUserService>();

        const string adminUsername = "admin";

        if (!await userService.UserExistsAsync(adminUsername))
        {
            var adminDto = new CreateUserDto(
                Username: adminUsername,
                Password: "YourAdminPassword123!",
                Role: "Admin"
            );

            await userService.CreateUserAsync(adminDto);
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