using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjektLABDetailing.Models
{
    public class Service
    {
        public int ServiceId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public decimal Price { get; set; }

        public virtual ICollection<OrderService> OrderServices { get; set; } = new List<OrderService>();
    }
}
