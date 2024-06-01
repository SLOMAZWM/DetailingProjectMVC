using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using ProjektLABDetailing.Models.User;
using Microsoft.EntityFrameworkCore;
using ProjektLABDetailing.Data;
using ProjektLABDetailing.Models;
using ProjektLABDetailing.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProjektLABDetailing.Controllers
{
    [Authorize(Roles = "Employee")]
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<User> _signInManager;

        public EmployeeController(ApplicationDbContext context, SignInManager<User> signInManager)
        {
            _context = context;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult EmployeeUserPanel()
        {
            var userType = HttpContext.Session.GetString("UserType");
            if (userType != "Employee")
            {
                return RedirectToAction("LoginRegister", "Account");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await _signInManager.SignOutAsync();
            return RedirectToAction("LoginRegister", "Account");
        }

        [HttpGet]
        public async Task<IActionResult> History()
        {
            var orderProducts = await _context.Orders
                .OfType<OrderProducts>()
                .Include(o => o.Client)
                    .ThenInclude(c => c.User)
                .Include(o => o.Car)
                .Include(o => o.Products)
                .ToListAsync();

            var orderTotals = orderProducts.ToDictionary(
                order => order.OrderId,
                order => order.Products.Sum(p => p.Price)
            );

            var model = new HistoryViewModel
            {
                OrderProducts = orderProducts,
                OrderTotals = orderTotals
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Services()
        {
            var orderServices = await _context.OrderServices
                .Include(os => os.Client)
                    .ThenInclude(c => c.User)
                .Include(os => os.Car)
                .Include(os => os.Services)
                .ToListAsync();

            var model = new ServicesViewModel
            {
                OrderServices = orderServices,
                StatusList = new List<string> { "Oczekuje", "W trakcie", "Zrealizowane" }
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var orderService = await _context.OrderServices.FindAsync(id);
            if (orderService == null)
            {
                return NotFound();
            }

            orderService.Status = status;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Services));
        }

        [HttpGet]
        public IActionResult AddService()
        {
            var model = new AddServiceViewModel
            {
                ServicesList = new List<SelectListItem>
                {
                    new SelectListItem { Value = "Mycie Detailingowe", Text = "Mycie Detailingowe" },
                    new SelectListItem { Value = "Regeneracja Lakieru", Text = "Regeneracja Lakieru" },
                    new SelectListItem { Value = "Powłoka Ceramiczna", Text = "Powłoka Ceramiczna" },
                    new SelectListItem { Value = "Detailing Wnętrza", Text = "Detailing Wnętrza" },
                    new SelectListItem { Value = "Regeneracja Reflektorów", Text = "Regeneracja Reflektorów" },
                    new SelectListItem { Value = "Naklejanie Folii", Text = "Naklejanie Folii" },
                    new SelectListItem { Value = "Przyciemnianie Szyb", Text = "Przyciemnianie Szyb" }
                }
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddService(AddServiceViewModel viewModel)
        {
            
                viewModel.ServicesList = GetServicesList();

            var user = new User
            {
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                Email = viewModel.Email,
                PhoneNumber = viewModel.PhoneNumber
            };

            var client = new Client
            {
                User = user
            };

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            var car = new Car
            {
                ClientId = client.ClientId,
                Brand = viewModel.Brand,
                Model = viewModel.Model,
                Year = viewModel.Year,
                Color = viewModel.Color,
                VIN = viewModel.VIN,
                Mileage = viewModel.Mileage
            };

            _context.Cars.Add(car);
            await _context.SaveChangesAsync();

            var userId = _signInManager.UserManager.GetUserId(User);
            var employee = await _context.Employees.SingleOrDefaultAsync(e => e.UserId == userId);

            if (employee == null)
            {
                ModelState.AddModelError("", "Bieżący użytkownik nie jest powiązany z pracownikiem.");
                viewModel.ServicesList = GetServicesList();
                return View(viewModel);
            }

            var orderService = new OrderService
            {
                Client = client,
                Car = car,
                ExecutionDate = viewModel.ExecutionDate,
                Status = "Oczekuje",
                Materials = viewModel.Materials,
                ClientRemarks = viewModel.ClientRemarks,
                EmployeeId = employee.EmployeeId 
            };

            _context.OrderServices.Add(orderService);
            await _context.SaveChangesAsync();

            ViewBag.SuccessMessage = "Usługa została dodana pomyślnie.";
            viewModel.ServicesList = GetServicesList();
            return View(viewModel);
        }

        private List<SelectListItem> GetServicesList()
        {
            return new List<SelectListItem>
    {
        new SelectListItem { Value = "Mycie Detailingowe", Text = "Mycie Detailingowe" },
        new SelectListItem { Value = "Regeneracja Lakieru", Text = "Regeneracja Lakieru" },
        new SelectListItem { Value = "Powłoka Ceramiczna", Text = "Powłoka Ceramiczna" },
        new SelectListItem { Value = "Detailing Wnętrza", Text = "Detailing Wnętrza" },
        new SelectListItem { Value = "Regeneracja Reflektorów", Text = "Regeneracja Reflektorów" },
        new SelectListItem { Value = "Naklejanie Folii", Text = "Naklejanie Folii" },
        new SelectListItem { Value = "Przyciemnianie Szyb", Text = "Przyciemnianie Szyb" }
    };
        }

    }
}
