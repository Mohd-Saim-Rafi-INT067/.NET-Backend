using System.ComponentModel.DataAnnotations;

namespace UserApi.Models;

public class User
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [MinLength(3,ErrorMessage = "Name must be at least 3 characters long")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;

}