using ProjektLABDetailing.Models.User;
using System.ComponentModel.DataAnnotations;

namespace ProjektLABDetailing.Models
{
    public class Car
    {
        public int CarId { get; set; }
        public int ClientId { get; set; }
        public Client Client { get; set; }

        [Required]
        public string Brand { get; set; }
        [Required]
        public string Model { get; set; }
        [Required]
        public int Year { get; set; }
        [Required]
        public string Color { get; set; }
        [Required]
        public string VIN { get; set; }
        [Required]
        public int Mileage { get; set; }
    }
}
