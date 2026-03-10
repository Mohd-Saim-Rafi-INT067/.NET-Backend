namespace AuthenticationAuthorization.DTOs;

public record RegisterDto(string Username, string Email, string Password);
public record LoginDto(string Email, string Password);

public record AuthResponseDto(string Token, string Username, string Email, string Role);
// Add these two:
public record ForgotPasswordDto(string Email);
public record ResetPasswordDto(string Token, string NewPassword);
