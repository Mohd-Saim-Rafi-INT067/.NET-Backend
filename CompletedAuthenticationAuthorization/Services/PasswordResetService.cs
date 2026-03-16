// Services/PasswordResetService.cs
using System.Security.Cryptography;
using AuthenticationAuthorization.DTOs;
using AuthenticationAuthorization.Interfaces;
using AuthenticationAuthorization.Models;

namespace AuthenticationAuthorization.Services;

public class PasswordResetService(
    IUserRepository userRepo,
    IPasswordResetTokenRepository resetTokenRepo,
    IRefreshTokenRepository refreshTokenRepo,
    EmailService emailService) : IPasswordResetService
{
    public async Task<ApiResponseDto> ForgotPasswordAsync(ForgotPasswordDto dto)
    {
        var user = await userRepo.GetByEmailAsync(dto.Email);

        if (user == null || !user.IsActive)
            return ApiResponseDto.Ok("If your email is registered you will receive a reset link!");

        // Invalidate any existing active tokens
        var existing = await resetTokenRepo.GetActiveByUserIdAsync(user.Id);
        foreach (var t in existing)
            t.IsUsed = true;

        var resetToken = GenerateResetToken();

        await resetTokenRepo.AddAsync(new PasswordResetToken
        {
            UserId = user.Id,
            Token = resetToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10)
        });

        await resetTokenRepo.SaveChangesAsync();
        await emailService.SendPasswordResetEmailAsync(user.Email, resetToken);

        return ApiResponseDto.Ok("If your email is registered you will receive a reset link!");
    }

    public async Task<ApiResponseDto> ResetPasswordAsync(ResetPasswordDto dto)
    {
        var resetToken = await resetTokenRepo.GetByTokenIncludeUserAsync(dto.Token);

        if (resetToken == null)
            return ApiResponseDto.Fail("Invalid reset token!");
        if (resetToken.IsUsed)
            return ApiResponseDto.Fail("Reset token already used!");
        if (resetToken.ExpiresAt < DateTime.UtcNow)
            return ApiResponseDto.Fail("Reset token has expired!");

        resetToken.User.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        resetToken.IsUsed = true;

        // Revoke all sessions — account may have been compromised
        var sessions = await refreshTokenRepo.GetActiveByUserIdAsync(resetToken.UserId);
        foreach (var t in sessions)
            t.IsRevoked = true;

        await resetTokenRepo.SaveChangesAsync();
        return ApiResponseDto.Ok("Password reset successfully! Please login again.");
    }

    public async Task<ApiResponseDto> ValidateResetTokenAsync(string token)
    {
        var resetToken = await resetTokenRepo.GetByTokenAsync(token);

        if (resetToken == null) return ApiResponseDto.Fail("Invalid reset token!");
        if (resetToken.IsUsed) return ApiResponseDto.Fail("Reset link already used!");
        if (resetToken.ExpiresAt < DateTime.UtcNow) return ApiResponseDto.Fail("Reset link has expired!");

        return ApiResponseDto.Ok("Token is valid!");
    }

    private static string GenerateResetToken()
    {
        var bytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes)
            .Replace("+", "-").Replace("/", "_").Replace("=", "");
    }
}