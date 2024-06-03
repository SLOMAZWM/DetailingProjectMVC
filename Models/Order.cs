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
        public int? EmployeeId { get; set; }
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
        public int OrderId { get; set; }
        public int ClientId { get; set; }
        public DateTime OrderDate { get; set; }

        [Required]
        public decimal TotalPrice { get; set; }

        [Required]
        public string Status { get; set; }

        [Required(ErrorMessage = "Imię jest wymagane.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Nazwisko jest wymagane.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Adres jest wymagany.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Miasto jest wymagane.")]
        public string City { get; set; }

        [Required(ErrorMessage = "Kod pocztowy jest wymagany.")]
        [RegularExpression(@"\d{2}-\d{3}", ErrorMessage = "Nieprawidłowy format kodu pocztowego.")]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = "Wybór sposobu płatności jest wymagany.")]
        public string PaymentMethod { get; set; }

        [Required(ErrorMessage = "Wybór sposobu dostawy jest wymagany.")]
        public string DeliveryMethod { get; set; }

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}