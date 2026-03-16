// Services/AuthService.cs
using AuthenticationAuthorization.DTOs;
using AuthenticationAuthorization.Interfaces;
using AuthenticationAuthorization.Models;

namespace AuthenticationAuthorization.Services;

public class AuthService(IUserRepository userRepo, ITokenService tokenService) : IAuthService
{
    public async Task<ApiResponseDto> RegisterAsync(RegisterDto dto)
    {
        if (await userRepo.EmailExistsAsync(dto.Email))
            return ApiResponseDto.Fail("Email already exists!");

        if (await userRepo.UsernameExistsAsync(dto.Username))
            return ApiResponseDto.Fail("Username already exists!");

        var newUser = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = "User"
        };

        await userRepo.AddAsync(newUser);
        await userRepo.SaveChangesAsync();
        return ApiResponseDto.Ok("User registered successfully!");
    }

    public async Task<ApiResponseDto<AuthResponseDto>> LoginAsync(LoginDto dto, HttpResponse response, string? deviceInfo)
    {
        var user = await userRepo.GetByEmailAsync(dto.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return ApiResponseDto<AuthResponseDto>.Fail("Invalid email or password!");

        if (!user.IsActive)
            return ApiResponseDto<AuthResponseDto>.Fail("Your account has been banned!");

        var accessToken = tokenService.GenerateAccessToken(user);
        var refreshToken = tokenService.GenerateRefreshToken();

        await tokenService.SaveRefreshTokenAsync(user.Id, refreshToken, deviceInfo);
        tokenService.SetRefreshTokenCookie(response, refreshToken);

        var data = new AuthResponseDto(accessToken, user.Username, user.Email, user.Role);
        return ApiResponseDto<AuthResponseDto>.Ok("Login successful!", data);
    }

    public async Task<ApiResponseDto<AuthResponseDto>> RefreshAsync(HttpRequest request, HttpResponse response)
    {
        var incomingToken = request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(incomingToken))
            return ApiResponseDto<AuthResponseDto>.Fail("No refresh token found!");

        if (await tokenService.IsReuseAttackAsync(incomingToken))
        {
            // Token was already used — potential theft. Nuke all sessions.
            var compromised = await tokenService.ValidateRefreshTokenAsync(incomingToken);
            if (compromised != null)
                await tokenService.RevokeAllTokensAsync(compromised.UserId);

            tokenService.ClearRefreshTokenCookie(response);
            return ApiResponseDto<AuthResponseDto>.Fail("Security alert! Please login again.");
        }

        var refreshToken = await tokenService.ValidateRefreshTokenAsync(incomingToken);
        if (refreshToken == null)
        {
            tokenService.ClearRefreshTokenCookie(response);
            return ApiResponseDto<AuthResponseDto>.Fail("Invalid or expired refresh token!");
        }

        if (!refreshToken.User.IsActive)
            return ApiResponseDto<AuthResponseDto>.Fail("Your account has been banned!");

        var newRefreshToken = await tokenService.RotateRefreshTokenAsync(refreshToken);
        var newAccessToken = tokenService.GenerateAccessToken(refreshToken.User);
        tokenService.SetRefreshTokenCookie(response, newRefreshToken);

        var data = new AuthResponseDto(
            newAccessToken,
            refreshToken.User.Username,
            refreshToken.User.Email,
            refreshToken.User.Role);

        return ApiResponseDto<AuthResponseDto>.Ok("Token refreshed successfully!", data);
    }

    public async Task<ApiResponseDto> LogoutAsync(HttpRequest request, HttpResponse response)
    {
        var token = request.Cookies["refreshToken"];
        if (!string.IsNullOrEmpty(token))
            await tokenService.RevokeTokenAsync(token);

        tokenService.ClearRefreshTokenCookie(response);
        return ApiResponseDto.Ok("Logged out successfully!");
    }

    public async Task<ApiResponseDto> LogoutAllAsync(int userId, HttpResponse response)
    {
        await tokenService.RevokeAllTokensAsync(userId);
        tokenService.ClearRefreshTokenCookie(response);
        return ApiResponseDto.Ok("Logged out from all devices successfully!");
    }
}