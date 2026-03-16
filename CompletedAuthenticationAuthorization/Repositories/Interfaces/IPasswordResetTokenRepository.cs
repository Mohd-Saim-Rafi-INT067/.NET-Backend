using AuthenticationAuthorization.Models;

namespace AuthenticationAuthorization.Interfaces;

public interface IPasswordResetTokenRepository
{
    Task<PasswordResetToken?> GetByTokenAsync(string token);//basic lookup
    Task<PasswordResetToken?> GetByTokenIncludeUserAsync(string token);//loads the user - used to update the password
    Task<List<PasswordResetToken>> GetActiveByUserIdAsync(int userId); //Gets all valid tokens to invalidate before issuing new one
    Task AddAsync(PasswordResetToken token);
    Task SaveChangesAsync();
}