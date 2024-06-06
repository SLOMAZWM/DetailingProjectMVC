using Microsoft.AspNetCore.Identity;
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

        public DbSet<Client> Clients { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<OrderService> OrderServices { get; set; }
        public DbSet<OrderProduct> OrderProducts { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<CarImage> CarImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Client>()
                .HasOne(c => c.User)
                .WithMany(u => u.Clients)
                .HasForeignKey(c => c.UserId);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.User)
                .WithMany(u => u.Employees)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<OrderService>()
                .HasMany(os => os.Services)
                .WithMany(s => s.OrderServices)
                .UsingEntity(j => j.ToTable("OrderServiceServices"));

            modelBuilder.Entity<OrderProduct>()
                .HasMany(op => op.Products)
                .WithMany(p => p.OrderProducts)
                .UsingEntity<Dictionary<string, object>>(
                    "OrderProductProducts",
                    j => j
                        .HasOne<Product>()
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .HasConstraintName("FK_OrderProductProducts_Products_ProductId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j
                        .HasOne<OrderProduct>()
                        .WithMany()
                        .HasForeignKey("OrderProductId")
                        .HasConstraintName("FK_OrderProductProducts_Order_OrderProductId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey("OrderProductId", "ProductId");
                        j.ToTable("OrderProductProducts");
                    });

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Service>()
                .Property(s => s.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<CartItem>()
                .Property(ci => ci.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Employee)
                .WithMany(e => e.Orders)
                .HasForeignKey(o => o.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CarImage>()
                .HasOne(ci => ci.Car)
                .WithMany(c => c.CarImages)
                .HasForeignKey(ci => ci.CarId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CarImage>()
                .Property(ci => ci.ImagePath)
                .IsRequired();

            modelBuilder.Entity<CarImage>()
                .Property(ci => ci.Title)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<CarImage>()
                .Property(ci => ci.Description)
                .HasMaxLength(500);
        }
    }
}
