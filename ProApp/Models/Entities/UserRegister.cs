using System.ComponentModel.DataAnnotations;

namespace ProApp.Models.Entities
{
    public class UserRegister
    {
        [Required(ErrorMessage = "The field Username is obligatory")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "The field Password is obligatory")]
        public string? Password { get; set; }
        [Required(ErrorMessage = "The field Confirm Password is obligatory")]
        public string? ConfirmPassword { get; set; }

    }
}
