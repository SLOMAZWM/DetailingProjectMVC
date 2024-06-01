using ProjektLABDetailing.Models.User;

namespace ProjektLABDetailing.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int ClientId { get; set; }
        public int EmployeeId { get; set; }
        public int? CarId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Discriminator { get; set; }
        public decimal ServicePrice { get; set; }

        public virtual Client Client { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual Car Car { get; set; }
    }
}
