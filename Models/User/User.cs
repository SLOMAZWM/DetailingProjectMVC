using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ProjektLABDetailing.Models.User
{
    public class User : IdentityUser
    {
        [Required(ErrorMessage = "Imię jest wymagane.")]
        [StringLength(50, ErrorMessage = "Imię musi mieć co najmniej 3 znaki i mniej niż 50 znaków.", MinimumLength = 3)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nazwisko jest wymagane.")]
        [StringLength(50, ErrorMessage = "Nazwisko musi mieć co najmniej 3 znaki i mniej niż 50 znaków.", MinimumLength = 3)]
        public string LastName { get; set; } = string.Empty;

        public UserRole Role { get; set; }

        public string FullName => $"{FirstName} {LastName}";
    }

    public enum UserRole
    {
        Client,
        Employee
    }
}
