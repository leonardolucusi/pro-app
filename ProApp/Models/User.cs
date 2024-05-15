using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ProApp.Models
{
    public class User
    {
        public int UserId { get; set; }
        [Required(ErrorMessage = "O campo Username é obrigatório.")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "O campo Password é obrigatório.")]
        public string? Password { get; set; }
        [HiddenInput(DisplayValue = false)]
        public string? RefreshToken { get; set; }
    }
}
