using System;
using System.Collections.Generic;

namespace ProjektLABDetailing.Models.ViewModels
{
    public class OrderDetailsViewModel
    {
        public int OrderId { get; set; }
        public string ClientName { get; set; }
        public string ClientEmail { get; set; }
        public string ClientPhoneNumber { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string PaymentMethod { get; set; }
        public string DeliveryMethod { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
        public List<OrderProductDetail> Products { get; set; }
    }

    public class OrderProductDetail
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
