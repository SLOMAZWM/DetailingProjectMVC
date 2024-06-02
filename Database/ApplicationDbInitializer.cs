using Microsoft.AspNetCore.Identity;
using ProjektLABDetailing.Data;
using ProjektLABDetailing.Models;
using ProjektLABDetailing.Models.User;
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
}
