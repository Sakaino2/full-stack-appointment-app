namespace Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? GoogleRefreshToken { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}