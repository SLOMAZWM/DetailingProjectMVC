using System.Collections.Generic;
using ProjektLABDetailing.Models;

namespace ProjektLABDetailing.Models.ViewModels
{
    public class HistoryViewModel
    {
        public IList<OrderProducts> OrderProducts { get; set; }
        public Dictionary<int, decimal> OrderTotals { get; set; } = new Dictionary<int, decimal>();
    }
}
