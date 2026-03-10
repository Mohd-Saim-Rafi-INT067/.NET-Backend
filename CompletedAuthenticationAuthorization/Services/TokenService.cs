using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using AuthenticationAuthorization.Data;
using AuthenticationAuthorization.Models;
using System.Security.Cryptography;

namespace AuthenticationAuthorization.Services;

public class TokenService(IConfiguration config, AppDbContext db)
{
    public string GenerateAccessToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:SecretKey"]!));

        var claims = new []
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            issuer : config["Jwt:Issuer"],
            audience : config["Jwt:Audience"],
            claims : claims,
            expires : DateTime.UtcNow.AddMinutes(15),
            signingCredentials : new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    public async Task SaveRefreshTokenAsync(int userId, string token, string? deviceInfo)
    {
        var refreshToken = new RefreshToken
        {
            UserId = userId,
            Token = token,
            DeviceInfo = deviceInfo,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };

        db.RefreshTokens.Add(refreshToken);
        await db.SaveChangesAsync();
    }

    public async Task<RefreshToken?> ValidateRefreshTokenAsync(string token)
    {
        var refreshToken = await db.RefreshTokens.Include(r=>r.User).FirstOrDefaultAsync(r=>r.Token == token);

        if (refreshToken == null) return null;
        if (refreshToken.IsRevoked) return null;
        if (refreshToken.IsUsed) return null; 
        if (refreshToken.ExpiresAt < DateTime.UtcNow) return null; 

        return refreshToken;
    }

    public async Task<string> RotateRefreshTokenAsync(RefreshToken oldToken)
    {
        oldToken.IsUsed = true; 
        db.RefreshTokens.Update(oldToken);

        var newToken = GenerateRefreshToken();
        await SaveRefreshTokenAsync(oldToken.UserId, newToken, oldToken.DeviceInfo);

        await db.SaveChangesAsync();
        return newToken;
    }

    public async Task<bool> isReuseAttackAsync(string token)
    {
        return await db.RefreshTokens.AnyAsync(r=>r.Token == token && r.IsUsed);
    }


    public async Task RevokeTokenAsync(string token)
    {
        var refreshToken = await db.RefreshTokens.FirstOrDefaultAsync(r=>r.Token == token);
        if (refreshToken != null)
        {
            refreshToken.IsRevoked = true;
            await db.SaveChangesAsync();
        }
    }

    public async Task RevokeAllTokenAsync(int userId)
    {
        var tokens = await db.RefreshTokens.Where(r=>r.UserId == userId && !r.IsRevoked).ToListAsync();
        foreach (var token in tokens)
        {
            token.IsRevoked = true;
        }
        await db.SaveChangesAsync();
    }

    public void SetRefreshTokenCookies(HttpResponse response, string token)
    {
        response.Cookies.Append("refreshToken", token, new CookieOptions{
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        });
    }
    public void ClearRefreshTokenCookies(HttpResponse response)
    {
        response.Cookies.Delete("refreshToken");
    }

}