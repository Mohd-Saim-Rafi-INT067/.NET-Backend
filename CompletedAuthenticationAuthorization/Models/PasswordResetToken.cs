namespace AuthenticationAuthorization.Models;

public class PasswordResetToken
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public User User { get; set; } = null!;


   // No IsRevoked here (unlike RefreshToken) because reset tokens are single-use by design. When a new reset is requested, all previous active tokens are marked IsUsed = true.
}