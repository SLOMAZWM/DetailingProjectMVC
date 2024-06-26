﻿using ProjektLABDetailing.Models.User;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public string Condition { get; set; }
    }

    public class OrderProduct : Order
    {
        [Required(ErrorMessage = "Imię jest wymagane.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Nazwisko jest wymagane.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Adres e-mail jest wymagany.")]
        [EmailAddress(ErrorMessage = "Nieprawidłowy format adresu e-mail.")]
        [RegularExpression(@"^[^@]+@[^@]+\.[^@]+$", ErrorMessage = "Adres e-mail musi zawierać dokładnie jeden znak '@'.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Numer telefonu jest wymagany.")]
        public string PhoneNumber { get; set; }

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

        public virtual ICollection<OrderProductDetail> OrderProductDetails { get; set; } = new List<OrderProductDetail>();

        [NotMapped]
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
    }

    public class OrderProductDetail
    {
        public int OrderProductDetailId { get; set; }
        public int OrderProductId { get; set; }
        public OrderProduct OrderProduct { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
    }
}
