using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjektLABDetailing.Models;
using ProjektLABDetailing.Models.User;

namespace ProjektLABDetailing.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Car> Cars { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderService> OrderServices { get; set; }
        public DbSet<OrderProducts> OrderProducts { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Service>()
                .Property(s => s.Price)
                .HasColumnType("decimal(18,2)");

            
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Client)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Employee)
                .WithMany(e => e.Orders)
                .HasForeignKey(o => o.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Car)
                .WithMany()
                .HasForeignKey(o => o.CarId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
