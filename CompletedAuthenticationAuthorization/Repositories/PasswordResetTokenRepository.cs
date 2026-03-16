// Repositories/PasswordResetTokenRepository.cs
using Microsoft.EntityFrameworkCore;
using AuthenticationAuthorization.Data;
using AuthenticationAuthorization.Interfaces;
using AuthenticationAuthorization.Models;

namespace AuthenticationAuthorization.Repositories;

public class PasswordResetTokenRepository(AppDbContext db) : IPasswordResetTokenRepository
{
    public Task<PasswordResetToken?> GetByTokenAsync(string token)
        => db.PasswordResetTokens.FirstOrDefaultAsync(t => t.Token == token);

    public Task<PasswordResetToken?> GetByTokenIncludeUserAsync(string token)
        => db.PasswordResetTokens.Include(t => t.User).FirstOrDefaultAsync(t => t.Token == token);

    public Task<List<PasswordResetToken>> GetActiveByUserIdAsync(int userId)
        => db.PasswordResetTokens
            .Where(t => t.UserId == userId && !t.IsUsed && t.ExpiresAt > DateTime.UtcNow)
            .ToListAsync();

    public async Task AddAsync(PasswordResetToken token)
        => await db.PasswordResetTokens.AddAsync(token);

    public Task SaveChangesAsync()
        => db.SaveChangesAsync();
}