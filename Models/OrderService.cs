using ProjektLABDetailing.Models;

public class OrderService : Order
{
    public DateTime? ExecutionDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Materials { get; set; } = string.Empty;
    public string ClientRemarks { get; set; } = string.Empty;
    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
}
