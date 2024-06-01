using System.Collections.Generic;

namespace ProjektLABDetailing.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public short Quantity { get; set; }
        public decimal Price { get; set; }
        public string ImgPath { get; set; } = string.Empty;

        public virtual ICollection<OrderProducts> OrderProducts { get; set; } = new List<OrderProducts>();
    }
}
