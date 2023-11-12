using System.ComponentModel.DataAnnotations;

namespace BlogWebAPI.Models.DTOs
{
    public class UserRegisterDto
    {
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = string.Empty;
    }
}
