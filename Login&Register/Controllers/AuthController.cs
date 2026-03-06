using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Login_Register.Models;
using Login_Register.Services;
using Login_Register.DTOs;

namespace Login_Register.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var (success,message) = await authService.RegisterAsync(dto);
        return success ? Ok(new {message}) : Conflict(new {message});  
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var (success, data, message ) = await authService.LoginAsync(dto);
        return success ? Ok (data) : Unauthorized(new {message});
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(new
        {
            Id = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
            Username = User.Identity?.Name,
            Email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
        });
    }
}