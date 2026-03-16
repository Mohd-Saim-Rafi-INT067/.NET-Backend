using System.ComponentModel.DataAnnotations;

namespace AuthenticationAuthorization.DTOs;

public record RegisterDto(
    [Required]
    [StringLength(30,MinimumLength = 3, ErrorMessage = "Username must be between 3 and 30 characters")]
    string Username,

    [Required]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    string Email, 

    [Required]
    [StringLength(72, MinimumLength = 8, ErrorMessage = "Password must be 8–72 characters")]
    string Password);
    


public record LoginDto(
    [Required][EmailAddress] string Email, [Required]string Password);



public record AuthResponseDto(string Token, string Username, string Email, string Role);


public record ForgotPasswordDto([Required][EmailAddress]string Email);


public record ResetPasswordDto([Required]string Token,
[Required][StringLength(72, MinimumLength = 8, ErrorMessage = "Password must be 8–72 characters")] string NewPassword);


public record ValidateResetTokenDto([Required]string Token);
