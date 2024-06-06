using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjektLABDetailing.Models.ViewModels
{
    public class AddCarImagesViewModel
    {
        public int CarId { get; set; }

        public string CarDetails { get; set; }

        [Required(ErrorMessage = "Proszę wybrać co najmniej jedno zdjęcie.")]
        public List<IFormFile> Images { get; set; }

        [Required(ErrorMessage = "Tytuł jest wymagany.")]
        [StringLength(100, ErrorMessage = "Tytuł może mieć maksymalnie 100 znaków.")]
        public string Title { get; set; }

        [StringLength(500, ErrorMessage = "Opis może mieć maksymalnie 500 znaków.")]
        public string Description { get; set; }
    }
}
