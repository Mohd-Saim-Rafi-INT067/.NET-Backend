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
        var result = await authService.RegisterAsync(dto);
        return result.Success ? Ok(result) : Conflict(result);  
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var result = await authService.LoginAsync(dto);
        return result.Success ? Ok(result) : Unauthorized(result);
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