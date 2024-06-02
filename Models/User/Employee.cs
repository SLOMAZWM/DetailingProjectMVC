using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjektLABDetailing.Models.User
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }

        [Required]
        public string Position { get; set; }

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
