namespace Application.Interfaces;

using Application.Dtos;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto dto, CancellationToken ct = default);
}