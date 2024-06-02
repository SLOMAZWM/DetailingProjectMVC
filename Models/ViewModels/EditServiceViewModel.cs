using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjektLABDetailing.Models.ViewModels
{
    public class EditServiceViewModel
    {
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Imię jest wymagane")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Nazwisko jest wymagane")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email jest wymagany")]
        [EmailAddress(ErrorMessage = "Nieprawidłowy format email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Numer telefonu jest wymagany")]
        [RegularExpression(@"^\+\d{1,3}\s?\d{1,3}\s?\d{3}\s?\d{3}\s?\d{3}$", ErrorMessage = "Numer telefonu musi być w formacie +00 000 000 000")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Marka jest wymagana")]
        public string Brand { get; set; }

        [Required(ErrorMessage = "Model jest wymagany")]
        public string Model { get; set; }

        [Required(ErrorMessage = "Rok jest wymagany")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Kolor jest wymagany")]
        public string Color { get; set; }

        [Required(ErrorMessage = "VIN jest wymagany")]
        public string VIN { get; set; }

        [Required(ErrorMessage = "Przebieg jest wymagany")]
        public int Mileage { get; set; }

        [Required(ErrorMessage = "Data realizacji jest wymagana")]
        public DateTime ExecutionDate { get; set; }

        [Required(ErrorMessage = "Wybór usługi jest wymagany")]
        public int SelectedServiceId { get; set; }

        [Required(ErrorMessage = "Materiały są wymagane")]
        public string Materials { get; set; }

        [Required(ErrorMessage = "Uwagi dla klienta są wymagane")]
        public string ClientRemarks { get; set; }

        public List<SelectListItem> ServicesList { get; set; }
    }
}
