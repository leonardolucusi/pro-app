using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ProApp.Models.Entities
{
    public class User
    {
        public int? UserId { get; set; }
        [Required(ErrorMessage = "The field Username is obligatory")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "The field Password is obligatory")]
        public string? Password { get; set; }
        [HiddenInput(DisplayValue = false)]
        public string? RefreshToken { get; set; }
    }
}
