using System.ComponentModel.DataAnnotations;

namespace ProjektLABDetailing.Models.User.ViewModels
{
    public class ChangeDataUserViewModel
    {
        public User CurrentUser { get; set; }
        public ChangePasswordViewModel ChangePasswordData { get; set; }
        public ChangeEmailViewModel ChangeEmailData { get; set; }
        public ChangePhoneNumberViewModel ChangePhoneNumberData { get; set; }
    }
}
