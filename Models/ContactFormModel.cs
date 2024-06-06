namespace ProjektLABDetailing.Models
{
    public class ContactFormModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string SelectedService { get; set; }
        public string Message { get; set; }
        public bool AcceptTerms { get; set; }
    }
}
