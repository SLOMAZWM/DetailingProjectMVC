using System;

namespace ProjektLABDetailing.Models.ViewModels
{
    public class OrderServiceViewModel
    {
        public int OrderId { get; set; }
        public string ClientName { get; set; }
        public string CarDetails { get; set; }
        public string ServiceName { get; set; }
        public string Status { get; set; }
        public DateTime ExecutionDate { get; set; }
        public string Materials { get; set; }
        public string ClientRemarks { get; set; }
    }
}
