using System.ComponentModel.DataAnnotations;

namespace UserApi.DTOs
{
    public class UserCreateDto
    {
        [Required(ErrorMessage = "Name is required")]
        [MinLength(3,ErrorMessage = "Name must be at least 3 characters long")]
        public string? Name { get; set; } 

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; } 
    }

    public class UserReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}