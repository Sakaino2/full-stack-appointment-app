namespace Application.Dtos;

public record LoginRequestDto(
    string Username,
    string Password
);

public record LoginResponseDto(
    string Token,
    string Username,
    DateTime ExpiresAt
);