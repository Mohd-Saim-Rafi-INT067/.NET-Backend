// Interfaces/IPasswordResetService.cs
using AuthenticationAuthorization.DTOs;

namespace AuthenticationAuthorization.Interfaces;

public interface IPasswordResetService
{
    Task<ApiResponseDto> ForgotPasswordAsync(ForgotPasswordDto dto);
    Task<ApiResponseDto> ResetPasswordAsync(ResetPasswordDto dto);
    Task<ApiResponseDto> ValidateResetTokenAsync(string token);
}