using System;
using System.Collections.Generic;
using ProjektLABDetailing.Models.User;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProjektLABDetailing.Models.ViewModels
{
    public class EditServiceViewModel
    {
        public int OrderId { get; set; }
        public Client Client { get; set; }
        public Car Car { get; set; }
        public DateTime ExecutionDate { get; set; }
        public string Status { get; set; }
        public string Materials { get; set; }
        public string ClientRemarks { get; set; }
        public List<string> Services { get; set; }
        public List<SelectListItem> ServicesList { get; set; } = new List<SelectListItem>();
        public List<string> StatusList { get; set; } = new List<string> { "Oczekuje", "W trakcie", "Zrealizowane", "Zakończone" };
    }
}
