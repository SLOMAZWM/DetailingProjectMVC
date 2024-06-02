using System.Collections.Generic;

namespace ProjektLABDetailing.Models.ViewModels
{
    public class HistoryViewModel
    {
        public IList<OrderProduct> OrderProducts { get; set; }
        public IList<OrderService> OrderServices { get; set; }
        public Dictionary<int, decimal> OrderTotals { get; set; } = new Dictionary<int, decimal>();
    }
}
