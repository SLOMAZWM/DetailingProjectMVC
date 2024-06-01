using System.Collections.Generic;

namespace ProjektLABDetailing.Models.User
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public string Position { get; set; } = string.Empty;

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
