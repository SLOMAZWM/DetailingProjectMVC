using System.Collections.Generic;

namespace ProjektLABDetailing.Models
{
    public class Service
    {
        public int ServiceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }

        public virtual ICollection<OrderService> OrderServices { get; set; } = new List<OrderService>();
    }
}
