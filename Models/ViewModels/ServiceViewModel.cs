using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace ProjektLABDetailing.Models.ViewModels
{
    public class ServicesViewModel
    {
        public IList<OrderService> OrderServices { get; set; }
        public List<string> StatusList { get; set; } = new List<string> { "Oczekuje", "W trakcie", "Zrealizowane" };
    }
}
