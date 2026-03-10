using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using AuthenticationAuthorization.Data;
using AuthenticationAuthorization.DTOs;
using AuthenticationAuthorization.Models;

namespace AuthenticationAuthorization.Services;

public class PasswordResetService(AppDbContext db, EmailService emailService)
{
    
    public async Task<ApiResponseDto> ForgotPasswordAsync(ForgotPasswordDto dto)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        
        if (user == null || !user.IsActive)
            return ApiResponseDto.Ok("If your email is registered you will receive a reset link!");

    
        var existingTokens = await db.PasswordResetTokens
            .Where(t => t.UserId == user.Id && !t.IsUsed && t.ExpiresAt > DateTime.UtcNow)
            .ToListAsync();

        foreach (var t in existingTokens)
            t.IsUsed = true;

        var resetToken = GenerateResetToken();

        db.PasswordResetTokens.Add(new PasswordResetToken
        {
            UserId = user.Id,
            Token = resetToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            IsUsed = false
        });

        await db.SaveChangesAsync();

        await emailService.SendPasswordResetEmailAsync(user.Email, resetToken);

        return ApiResponseDto.Ok("If your email is registered you will receive a reset link!");
    }

    
    public async Task<ApiResponseDto> ResetPasswordAsync(ResetPasswordDto dto)
    {
        
        var resetToken = await db.PasswordResetTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == dto.Token);

      
        if (resetToken == null)
            return ApiResponseDto.Fail("Invalid reset token!");

        if (resetToken.IsUsed)
            return ApiResponseDto.Fail("Reset token already used!");

        if (resetToken.ExpiresAt < DateTime.UtcNow)
            return ApiResponseDto.Fail("Reset token has expired!");

        
        resetToken.User.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

      
        resetToken.IsUsed = true;

        
        var allRefreshTokens = await db.RefreshTokens
            .Where(r => r.UserId == resetToken.UserId && !r.IsRevoked)
            .ToListAsync();

        foreach (var token in allRefreshTokens)
            token.IsRevoked = true;

        await db.SaveChangesAsync();

        return ApiResponseDto.Ok("Password reset successfully! Please login again.");
    }

    public async Task<ApiResponseDto> ValidateResetTokenAsync(string token)
    {
        var resetToken = await db.PasswordResetTokens
            .FirstOrDefaultAsync(t => t.Token == token);

        if (resetToken == null)
            return ApiResponseDto.Fail("Invalid reset token!");

        if (resetToken.IsUsed)
            return ApiResponseDto.Fail("Reset link already used!");

        if (resetToken.ExpiresAt < DateTime.UtcNow)
            return ApiResponseDto.Fail("Reset link has expired!");

        return ApiResponseDto.Ok("Token is valid!");
    }

   
    private string GenerateResetToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", ""); 
    }
}