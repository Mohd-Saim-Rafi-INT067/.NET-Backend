using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuthenticationAuthorization.DTOs;
using AuthenticationAuthorization.Services;

namespace AuthenticationAuthorization.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AuthService authService, PasswordResetService passwordResetService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var result = await authService.RegisterAsync(dto);
        return result.Success ? Ok(result) : Conflict(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var deviceInfo = Request.Headers["User-Agent"].ToString();
        var result = await authService.LoginAsync(dto, Response, deviceInfo);
        return result.Success ? Ok(result) : Unauthorized(result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var result = await authService.RefreshAsync(Request, Response);
        return result.Success ? Ok(result) : Unauthorized(result);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var result = await authService.LogoutAsync(Request, Response);
        return Ok(result);
    }

    [Authorize]
    [HttpPost("logout-all")]
    public async Task<IActionResult> LogoutAll()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await authService.LogoutAllAsync(userId, Response);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(new
        {
            Id = User.FindFirstValue(ClaimTypes.NameIdentifier),
            Username = User.FindFirstValue(ClaimTypes.Name),
            Email = User.FindFirstValue(ClaimTypes.Email),
            Role = User.FindFirstValue(ClaimTypes.Role)
        });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
    {
        var result = await passwordResetService.ForgotPasswordAsync(dto);
        return Ok(result);
    }

    [HttpGet("validate-reset-token")]
    public async Task<IActionResult> ValidateResetToken(string token)
    {
        var result = await passwordResetService.ValidateResetTokenAsync(token);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
    {
        var result = await passwordResetService.ResetPasswordAsync(dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}