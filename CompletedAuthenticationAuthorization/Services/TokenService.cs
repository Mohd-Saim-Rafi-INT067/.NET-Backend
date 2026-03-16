// Services/TokenService.cs
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using AuthenticationAuthorization.Interfaces;
using AuthenticationAuthorization.Models;

namespace AuthenticationAuthorization.Services;

public class TokenService(IOptions<JwtOptions> jwtOptions, IRefreshTokenRepository refreshTokenRepo) : ITokenService
{
    private readonly JwtOptions _jwt = jwtOptions.Value;

    public string GenerateAccessToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SecretKey));
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwt.AccessTokenMinutes),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var bytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    public async Task SaveRefreshTokenAsync(int userId, string token, string? deviceInfo)
    {
        var refreshToken = new RefreshToken
        {
            UserId = userId,
            Token = token,
            DeviceInfo = deviceInfo,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwt.RefreshTokenDays),
            CreatedAt = DateTime.UtcNow
        };

        await refreshTokenRepo.AddAsync(refreshToken);
        await refreshTokenRepo.SaveChangesAsync();
    }

    public async Task<RefreshToken?> ValidateRefreshTokenAsync(string token)
    {
        var refreshToken = await refreshTokenRepo.GetByTokenIncludeUserAsync(token);

        if (refreshToken == null) return null;
        if (refreshToken.IsRevoked) return null;
        if (refreshToken.IsUsed) return null;
        if (refreshToken.ExpiresAt < DateTime.UtcNow) return null;

        return refreshToken;
    }

    public async Task<string> RotateRefreshTokenAsync(RefreshToken oldToken)
    {
        oldToken.IsUsed = true;
        var newToken = GenerateRefreshToken();
        await SaveRefreshTokenAsync(oldToken.UserId, newToken, oldToken.DeviceInfo);
        await refreshTokenRepo.SaveChangesAsync();
        return newToken;
    }

    public Task<bool> IsReuseAttackAsync(string token)
        => refreshTokenRepo.IsTokenUsedAsync(token);

    public async Task RevokeTokenAsync(string token)
    {
        var refreshToken = await refreshTokenRepo.GetByTokenAsync(token);
        if (refreshToken == null) return;
        refreshToken.IsRevoked = true;
        await refreshTokenRepo.SaveChangesAsync();
    }

    public async Task RevokeAllTokensAsync(int userId)
    {
        var tokens = await refreshTokenRepo.GetActiveByUserIdAsync(userId);
        foreach (var t in tokens)
            t.IsRevoked = true;
        await refreshTokenRepo.SaveChangesAsync();
    }

    public void SetRefreshTokenCookie(HttpResponse response, string token)
    {
        response.Cookies.Append("refreshToken", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(_jwt.RefreshTokenDays)
        });
    }

    public void ClearRefreshTokenCookie(HttpResponse response)
    {
        response.Cookies.Append("refreshToken", "", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(-1)
        });
    }
}