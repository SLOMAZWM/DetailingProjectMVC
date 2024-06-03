using System.Collections.Generic;

namespace ProjektLABDetailing.Models.User
{
    public class Client
    {
        public int ClientId { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public virtual ICollection<Car> Cars { get; set; } = new List<Car>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
