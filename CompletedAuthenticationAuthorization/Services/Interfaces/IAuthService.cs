// Interfaces/IAuthService.cs
using AuthenticationAuthorization.DTOs;

namespace AuthenticationAuthorization.Interfaces;

public interface IAuthService
{
    Task<ApiResponseDto> RegisterAsync(RegisterDto dto);
    Task<ApiResponseDto<AuthResponseDto>> LoginAsync(LoginDto dto, HttpResponse response, string? deviceInfo);
    Task<ApiResponseDto<AuthResponseDto>> RefreshAsync(HttpRequest request, HttpResponse response);
    Task<ApiResponseDto> LogoutAsync(HttpRequest request, HttpResponse response);
    Task<ApiResponseDto> LogoutAllAsync(int userId, HttpResponse response);
}