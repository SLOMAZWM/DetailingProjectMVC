using System.Collections.Generic;

namespace ProjektLABDetailing.Models.ViewModels
{
    public class OrderProductsViewModel
    {
        public List<OrderProducts> OrderProducts { get; set; }
        public Dictionary<int, decimal> OrderTotals { get; set; }
        public List<string> StatusList { get; set; } = new List<string> { "Nowe", "W realizacji", "Wysłane", "Zakończone" };
    }
}
