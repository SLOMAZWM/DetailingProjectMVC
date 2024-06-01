using System.ComponentModel.DataAnnotations;

namespace ProjektLABDetailing.Models.User.ViewModels
{
    public class LoginUserViewModel
    {
        [Required(ErrorMessage = "Email jest wymagany.")]
        [EmailAddress(ErrorMessage = "Nieprawidłowy format email.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Hasło jest wymagane.")]
        public string Password { get; set; }
    }
}
