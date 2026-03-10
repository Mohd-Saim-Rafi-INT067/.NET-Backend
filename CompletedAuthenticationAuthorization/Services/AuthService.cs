using Microsoft.EntityFrameworkCore;
using AuthenticationAuthorization.Data;
using AuthenticationAuthorization.DTOs;
using AuthenticationAuthorization.Models;

namespace AuthenticationAuthorization.Services;

public class AuthService(AppDbContext db, TokenService tokenService)
{
    public async Task<ApiResponseDto> RegisterAsync(RegisterDto dto)
    {
        if (await db.Users.AnyAsync(u=>u.Email == dto.Email))
        {
            return ApiResponseDto.Fail("Email already exists!");
        }
        if (await db.Users.AnyAsync(u=>u.Username == dto.Username))
        {
            return ApiResponseDto.Fail("Username already exists!");
        }
        var newUser = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = "User"
        };
        db.Users.Add(newUser);
        await db.SaveChangesAsync();
        return ApiResponseDto.Ok("User registered successfully!");
    }

    public async Task<ApiResponseDto<AuthResponseDto>> LoginAsync(LoginDto dto, HttpResponse response, string? deviceInfo)
    {
        var user = await db.Users.FirstOrDefaultAsync(u=>u.Email == dto.Email);
        
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            return ApiResponseDto<AuthResponseDto>.Fail("Invalid email or password!");
        }

        if (!user.IsActive)
        {
            return ApiResponseDto<AuthResponseDto>.Fail("Your account is banned!");
        }

        var accessToken = tokenService.GenerateAccessToken(user);
        var refreshToken = tokenService.GenerateRefreshToken();
        await tokenService.SaveRefreshTokenAsync(user.Id, refreshToken, deviceInfo);

        tokenService.SetRefreshTokenCookies(response, refreshToken);

        var data = new AuthResponseDto(accessToken, user.Username, user.Email, user.Role);
        return ApiResponseDto<AuthResponseDto>.Ok("Login successful!", data);
    }

    public async Task<ApiResponseDto<AuthResponseDto>> RefreshAsync(HttpRequest request, HttpResponse response)
    {
        var incomingToken = request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(incomingToken))
        {
            return ApiResponseDto<AuthResponseDto>.Fail("No refresh token found!");
        }

        if (await tokenService.isReuseAttackAsync(incomingToken))
        {
            //hacker
            var compromisedToken = await db.RefreshTokens.SingleOrDefaultAsync(t=>t.Token == incomingToken);
            if (compromisedToken != null)
            {
                await tokenService.RevokeAllTokenAsync(compromisedToken.UserId);
            }
            tokenService.ClearRefreshTokenCookies(response);
            return ApiResponseDto<AuthResponseDto>.Fail("Security alert! Please login again.");
        }

        //validate the token
        var refreshToken = await tokenService.ValidateRefreshTokenAsync(incomingToken);
        if (refreshToken == null)
        {
            tokenService.ClearRefreshTokenCookies(response);
            return ApiResponseDto<AuthResponseDto>.Fail("Invalid refresh token! Please login again.");
        }

        if (!refreshToken.User.IsActive)
        {
            return ApiResponseDto<AuthResponseDto>.Fail("Your account is banned!");
        }

        //rotate the token
        var newRefreshToken = await tokenService.RotateRefreshTokenAsync(refreshToken);
        var newAccessToken = tokenService.GenerateAccessToken(refreshToken.User);
        tokenService.SetRefreshTokenCookies(response, newRefreshToken);

        var data = new AuthResponseDto(
            newAccessToken, 
            refreshToken.User.Username, 
            refreshToken.User.Email, 
            refreshToken.User.Role
        );

        return ApiResponseDto<AuthResponseDto>.Ok("Token refreshed successfully!", data);
    }

    //Logout single device
    public async Task<ApiResponseDto> LogoutAsync(HttpRequest request, HttpResponse response)
    {
        var token = request.Cookies["refreshToken"];
        if (!string.IsNullOrEmpty(token))
        {
            await tokenService.RevokeTokenAsync(token);
        }
        tokenService.ClearRefreshTokenCookies(response);
        return ApiResponseDto.Ok("Logged out successfully!");
    }

    public async Task<ApiResponseDto> LogoutAllAsync(int userId, HttpResponse response)
    {
        await tokenService.RevokeAllTokenAsync(userId);
        tokenService.ClearRefreshTokenCookies(response);
        return ApiResponseDto.Ok("Logged out from all devices successfully!");
    }


}