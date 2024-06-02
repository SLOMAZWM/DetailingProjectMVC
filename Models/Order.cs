using ProjektLABDetailing.Models.User;
using System;
using System.Collections.Generic;

namespace ProjektLABDetailing.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int ClientId { get; set; }
        public int EmployeeId { get; set; }
        public int? CarId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal ServicePrice { get; set; }
        public string Status { get; set; }
        public string Address { get; set; }

        public int? ServiceId { get; set; }
        public virtual Service Service { get; set; }

        public virtual Client Client { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual Car Car { get; set; }
    }
}
