namespace Infrastructure.Services;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Infrastructure.Auth;
using Application.Dtos;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

public class AuthService(IApplicationDbContext context, IOptions<JwtSettings> jwtOptions) : IAuthService
{
    private readonly IApplicationDbContext _context = context;
    private readonly JwtSettings _jwtSettings = jwtOptions.Value;

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto dto, CancellationToken ct = default)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == dto.Username, ct);

        if (user is null) return null;

        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
        if (!isPasswordValid) return null;

        var expiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);
        var token = GenerateJwtToken(user.Id.ToString(), user.Username, user.Role, expiresAtUtc);

        return new LoginResponseDto(token, user.Username, expiresAtUtc);
    }

    private string GenerateJwtToken(string userId, string username, string role, DateTime expiresAtUtc)
    {
        var key = Encoding.UTF8.GetBytes(_jwtSettings.Secret);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role ?? "Admin")
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiresAtUtc,
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}