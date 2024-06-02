using ProjektLABDetailing.Models.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjektLABDetailing.Models
{
    public abstract class Order
    {
        public int OrderId { get; set; }
        public int ClientId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime OrderDate { get; set; }

        [Required]
        public decimal TotalPrice { get; set; }

        [Required]
        public string Status { get; set; }

        public virtual Client Client { get; set; }
        public virtual Employee Employee { get; set; }
    }

    public class OrderService : Order
    {
        public DateTime? ExecutionDate { get; set; }
        public string Materials { get; set; } = string.Empty;
        public string ClientRemarks { get; set; } = string.Empty;
        public virtual ICollection<Service> Services { get; set; } = new List<Service>();
        public int? CarId { get; set; }
        public virtual Car Car { get; set; }
    }

    public class OrderProduct : Order
    {
        [Required]
        public string Address { get; set; }
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
