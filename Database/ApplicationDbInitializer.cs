using Microsoft.AspNetCore.Identity;
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
}
