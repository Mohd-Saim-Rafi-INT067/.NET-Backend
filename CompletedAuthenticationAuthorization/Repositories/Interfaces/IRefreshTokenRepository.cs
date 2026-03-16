using AuthenticationAuthorization.Models;

namespace AuthenticationAuthorization.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token); //basic lookup
    Task<RefreshToken?> GetByTokenIncludeUserAsync(string token); //lookup with user loaded
    Task<List<RefreshToken>> GetActiveByUserIdAsync(int userId); //all non-revoked tokens for a user
    Task<bool> IsTokenUsedAsync(string token);
    Task AddAsync(RefreshToken token);
    Task SaveChangesAsync();
}