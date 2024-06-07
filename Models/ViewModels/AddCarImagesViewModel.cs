using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjektLABDetailing.Models.ViewModels
{
    public class AddCarImagesViewModel
    {
        public int CarId { get; set; }

        [Required]
        public string Title { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Wybierz zdjęcia")]
        public List<IFormFile> Images { get; set; }

        public string CarDetails { get; set; }
    }
}
