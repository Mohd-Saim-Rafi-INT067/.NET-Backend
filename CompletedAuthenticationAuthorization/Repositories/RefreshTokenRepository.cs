// Repositories/RefreshTokenRepository.cs
using Microsoft.EntityFrameworkCore;
using AuthenticationAuthorization.Data;
using AuthenticationAuthorization.Interfaces;
using AuthenticationAuthorization.Models;

namespace AuthenticationAuthorization.Repositories;

public class RefreshTokenRepository(AppDbContext db) : IRefreshTokenRepository
{
    public Task<RefreshToken?> GetByTokenAsync(string token)
        => db.RefreshTokens.FirstOrDefaultAsync(r => r.Token == token);

    public Task<RefreshToken?> GetByTokenIncludeUserAsync(string token)
        => db.RefreshTokens.Include(r => r.User).FirstOrDefaultAsync(r => r.Token == token);

    public Task<List<RefreshToken>> GetActiveByUserIdAsync(int userId)
        => db.RefreshTokens
            .Where(r => r.UserId == userId && !r.IsRevoked)
            .ToListAsync();

    public Task<bool> IsTokenUsedAsync(string token)
        => db.RefreshTokens.AnyAsync(r => r.Token == token && r.IsUsed);

    public async Task AddAsync(RefreshToken token)
        => await db.RefreshTokens.AddAsync(token);

    public Task SaveChangesAsync()
        => db.SaveChangesAsync();
}