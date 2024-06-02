using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace ProjektLABDetailing.Models.ViewModels
{
    public class ServicesViewModel
    {
        public List<SelectListItem> ServicesList { get; set; }
        public int SelectedServiceId { get; set; }
        public List<OrderServiceViewModel> OrderServices { get; set; }
        public List<string> StatusList { get; set; }
    }
}
