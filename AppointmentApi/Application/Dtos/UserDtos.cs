namespace Application.Dtos;

public record CreateUserDto(
    string Username,
    string Password,
    string Role
);

public record UserResponseDto(
    Guid Id,
    string Username,
    string Role,
    DateTime CreatedAtUtc
);