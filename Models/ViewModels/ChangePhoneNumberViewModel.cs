using System.ComponentModel.DataAnnotations;

namespace ProjektLABDetailing.Models.User.ViewModels
{
    public class ChangePhoneNumberViewModel
    {
        [Required(ErrorMessage = "Nowy numer telefonu jest wymagany.")]
        [RegularExpression(@"^\+\d{1,3}\s?\d{1,3}\s?\d{3}\s?\d{3}\s?\d{3}$", ErrorMessage = "Numer telefonu musi być w formacie +00 000 000 000.")]
        public string NewPhoneNumber { get; set; }

        [Required(ErrorMessage = "Potwierdzenie numeru telefonu jest wymagane.")]
        [Compare("NewPhoneNumber", ErrorMessage = "Numery telefonów nie są zgodne.")]
        public string ConfirmPhoneNumber { get; set; }
    }
}
