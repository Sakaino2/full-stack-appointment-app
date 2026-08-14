namespace Application.Interfaces;

using Application.Dtos;

public interface IUserService
{
    Task<UserResponseDto> CreateUserAsync(CreateUserDto dto, CancellationToken ct = default);
    Task<bool> UserExistsAsync(string username, CancellationToken ct = default);
}