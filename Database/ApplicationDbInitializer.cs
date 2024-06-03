using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjektLABDetailing.Data;
using ProjektLABDetailing.Models;
using ProjektLABDetailing.Models.User;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public static class ApplicationDbInitializer
{
    public static async Task SeedRolesAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        if (!await roleManager.RoleExistsAsync("Employee"))
        {
            await roleManager.CreateAsync(new IdentityRole("Employee"));
        }
        if (!await roleManager.RoleExistsAsync("Client"))
        {
            await roleManager.CreateAsync(new IdentityRole("Client"));
        }

        var user1 = new User
        {
            Id = "43abbeaf-ad9b-4af9-ad70-21e247d541bf",
            FirstName = "Michał",
            LastName = "Falczykowski",
            UserName = "fajnykolega@gmail.com",
            NormalizedUserName = "FAJNYKOLEGA@GMAIL.COM",
            Email = "fajnykolega@gmail.com",
            NormalizedEmail = "FAJNYKOLEGA@GMAIL.COM",
            EmailConfirmed = false,
            PhoneNumber = "+48 570 213 465",
            PhoneNumberConfirmed = false,
            TwoFactorEnabled = false,
            LockoutEnabled = true,
            AccessFailedCount = 0
        };

        var user2 = new User
        {
            Id = "4cafd197-8c2c-41da-95bd-68291e84733f",
            FirstName = "Janusz",
            LastName = "Cebulasz",
            UserName = "fajnypracownik@gmail.com",
            NormalizedUserName = "FAJNYPRACOWNIK@GMAIL.COM",
            Email = "fajnypracownik@gmail.com",
            NormalizedEmail = "FAJNYPRACOWNIK@GMAIL.COM",
            EmailConfirmed = false,
            PhoneNumber = "+48 570 213 456",
            PhoneNumberConfirmed = false,
            TwoFactorEnabled = false,
            LockoutEnabled = true,
            AccessFailedCount = 0
        };

        if (userManager.Users.All(u => u.Id != user1.Id))
        {
            await userManager.CreateAsync(user1, "Testy321!PL");
            await userManager.AddToRoleAsync(user1, "Client");
        }

        if (userManager.Users.All(u => u.Id != user2.Id))
        {
            await userManager.CreateAsync(user2, "Testy321!PL");
            await userManager.AddToRoleAsync(user2, "Employee");
        }
    }

    public static void SeedServices(ApplicationDbContext context)
    {
        if (!context.Services.Any())
        {
            var services = new List<Service>
            {
                new Service { Name = "Mycie zewnętrzne", Description = "Kompletne mycie zewnętrzne i woskowanie", Price = 29.99m },
                new Service { Name = "Czyszczenie wnętrza", Description = "Dokładne czyszczenie wnętrza", Price = 49.99m },
                new Service { Name = "Pełne Detailing", Description = "Kompletne detailing wnętrza i zewnętrza", Price = 99.99m },
                new Service { Name = "Czyszczenie silnika", Description = "Czyszczenie komory silnika", Price = 59.99m },
                new Service { Name = "Czyszczenie kół", Description = "Szczegółowe czyszczenie kół i opon", Price = 19.99m },
                new Service { Name = "Czyszczenie szyb", Description = "Czyszczenie i polerowanie wszystkich szyb", Price = 14.99m },
                new Service { Name = "Ochrona lakieru", Description = "Aplikacja ochronnego wosku lub uszczelniacza", Price = 79.99m },
                new Service { Name = "Pielęgnacja skóry", Description = "Czyszczenie i konserwacja skórzanych foteli", Price = 39.99m },
                new Service { Name = "Usuwanie zapachów", Description = "Usuwanie nieprzyjemnych zapachów z wnętrza", Price = 24.99m },
                new Service { Name = "Renowacja reflektorów", Description = "Renowacja zmatowiałych lub zażółkłych reflektorów", Price = 34.99m },
                new Service { Name = "Pakiet Full PPF", Description = "To całościowe zabezpieczenie samochodu folią ochronną PPF", Price = 15000.0m},
                new Service { Name = "Pakiet Full Front", Description = "To całkowite zabezpieczenie przedniej części auta folią ochronną PPF", Price = 15000.0m},
                new Service { Name = "Pakiet Front", Description = "Podstawowy pakiet ochronny folią ochronną PPF", Price = 15000.0m},
            };

            context.Services.AddRange(services);
            context.SaveChanges();
        }
    }

    public static void SeedProducts(ApplicationDbContext context)
    {
        if (!context.Products.Any())
        {
            context.Database.OpenConnection();
            try
            {
                context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[Products] ON");

                var products = new List<Product>
                {
                    new Product { ProductId = 2, Name = "Quick Detailer", Quantity = 10, Price = 79.99m, ImgPath = "Pictures/Quick.png" },
                    new Product { ProductId = 3, Name = "All Purpose Cleaner", Quantity = 10, Price = 99.99m, ImgPath = "Pictures/APC.png" },
                    new Product { ProductId = 4, Name = "Glass Cleaner", Quantity = 10, Price = 59.99m, ImgPath = "Pictures/Glass.png" },
                    new Product { ProductId = 5, Name = "Wosk Ochronny", Quantity = 10, Price = 49.99m, ImgPath = "Pictures/Wax.png" },
                    new Product { ProductId = 6, Name = "Rekawice Aplikatora Black - niepylące", Quantity = 10, Price = 9.99m, ImgPath = "Pictures/RekawiceAplikatoraBlack.png" },
                    new Product { ProductId = 7, Name = "Meguiar's Gold Class Car Wash", Quantity = 10, Price = 55.99m, ImgPath = "Pictures/GoldClassWash.png" }
                };

                context.Products.AddRange(products);
                context.SaveChanges();

                context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[Products] OFF");
            }
            finally
            {
                context.Database.CloseConnection();
            }
        }
    }
}
