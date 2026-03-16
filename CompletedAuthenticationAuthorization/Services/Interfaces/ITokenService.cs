// Interfaces/ITokenService.cs
using AuthenticationAuthorization.Models;

namespace AuthenticationAuthorization.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    Task SaveRefreshTokenAsync(int userId, string token, string? deviceInfo);
    Task<RefreshToken?> ValidateRefreshTokenAsync(string token);
    Task<string> RotateRefreshTokenAsync(RefreshToken oldToken);
    Task<bool> IsReuseAttackAsync(string token);
    Task RevokeTokenAsync(string token);
    Task RevokeAllTokensAsync(int userId);
    void SetRefreshTokenCookie(HttpResponse response, string token);
    void ClearRefreshTokenCookie(HttpResponse response);
}