using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ProjektLABDetailing.Models
{
    public class CartItem
    {
        [Key]
        public int CartItemId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
    }
}
