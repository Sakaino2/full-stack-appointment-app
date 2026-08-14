namespace Infrastructure.Services;

using Application.Dtos;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

public class UserService : IUserService
{
    private readonly IApplicationDbContext _context;

    public UserService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> UserExistsAsync(string username, CancellationToken ct = default)
    {
        return await _context.Users.AnyAsync(u => u.Username == username, ct);
    }

    public async Task<UserResponseDto> CreateUserAsync(CreateUserDto dto, CancellationToken ct = default)
    {
        if (await UserExistsAsync(dto.Username, ct))
        {
            throw new InvalidOperationException($"User with username '{dto.Username}' already exists.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = dto.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = string.IsNullOrWhiteSpace(dto.Role) ? "Admin" : dto.Role,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(ct);

        return new UserResponseDto(user.Id, user.Username, user.Role, user.CreatedAt);
    }
}