namespace AuthenticationAuthorization.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty; //random 64 bytes stored in cookies and db, not JWT
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRevoked { get; set; } = false; //logout/ban
        public bool IsUsed { get; set; } = false; //if this is true (reuse attack)

        public string? DeviceInfo { get; set; }
        
        //Foreign Key and Navigation Property
        public int UserId { get; set; }
        public User User { get; set; } = null!;

    }
}