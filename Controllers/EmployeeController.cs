﻿using Microsoft.AspNetCore.Mvc;
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
using System.Linq;

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
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public async Task<IActionResult> History()
        {
            var orderProducts = await _context.Orders
                .OfType<OrderProducts>()
                .Where(op => op.Status == "Zakończone")
                .Include(o => o.Client)
                    .ThenInclude(c => c.User)
                .Include(o => o.Car)
                .Include(o => o.Products)
                .ToListAsync();

            var orderServices = await _context.OrderServices
                .Where(os => os.Status == "Zakończone")
                .Include(os => os.Client)
                    .ThenInclude(c => c.User)
                .Include(os => os.Car)
                .Include(os => os.Services)
                .ToListAsync();

            var orderTotals = orderProducts.ToDictionary(
                order => order.OrderId,
                order => order.Products.Sum(p => p.Price)
            );

            var model = new HistoryViewModel
            {
                OrderProducts = orderProducts,
                OrderServices = orderServices,
                OrderTotals = orderTotals
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Services()
        {
            var orderServices = await _context.OrderServices
                .Where(os => os.Status != "Zakończone")
                .Include(os => os.Client)
                    .ThenInclude(c => c.User)
                .Include(os => os.Car)
                .Include(os => os.Services)
                .ToListAsync();

            var model = new ServicesViewModel
            {
                OrderServices = orderServices,
                StatusList = new List<string> { "Oczekuje", "W trakcie", "Zrealizowane", "Zakończone" }
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
                ServicesList = GetServicesList()
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

        [HttpPost]
        public async Task<IActionResult> DeleteService(int id)
        {
            var orderService = await _context.OrderServices.FindAsync(id);
            if (orderService == null)
            {
                return NotFound();
            }

            _context.OrderServices.Remove(orderService);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Services));
        }

        [HttpGet]
        public async Task<IActionResult> EditService(int id)
        {
            var orderService = await _context.OrderServices
                .Include(os => os.Client)
                    .ThenInclude(c => c.User)
                .Include(os => os.Car)
                .Include(os => os.Services)
                .FirstOrDefaultAsync(os => os.OrderId == id);

            if (orderService == null)
            {
                return NotFound();
            }

            var model = new AddServiceViewModel
            {
                FirstName = orderService.Client.User.FirstName,
                LastName = orderService.Client.User.LastName,
                Email = orderService.Client.User.Email,
                PhoneNumber = orderService.Client.User.PhoneNumber,
                Brand = orderService.Car.Brand,
                Model = orderService.Car.Model,
                Year = orderService.Car.Year,
                Color = orderService.Car.Color,
                VIN = orderService.Car.VIN,
                Mileage = orderService.Car.Mileage,
                ExecutionDate = orderService.ExecutionDate,
                SelectedService = orderService.Services.FirstOrDefault()?.Name,
                Materials = orderService.Materials,
                ClientRemarks = orderService.ClientRemarks,
                ServicesList = GetServicesList()
            };

            return View("AddService", model);
        }

        [HttpPost]
        public async Task<IActionResult> EditService(AddServiceViewModel viewModel)
        {
            var orderService = await _context.OrderServices
                .Include(os => os.Client)
                    .ThenInclude(c => c.User)
                .Include(os => os.Car)
                .Include(os => os.Services)
                .FirstOrDefaultAsync(os => os.OrderId == viewModel.OrderId);
            if (orderService == null)
            {
                return NotFound();
            }

            orderService.ExecutionDate = viewModel.ExecutionDate;
            orderService.Status = "Oczekuje";
            orderService.Materials = viewModel.Materials;
            orderService.ClientRemarks = viewModel.ClientRemarks;

            // Update the client information
            orderService.Client.User.FirstName = viewModel.FirstName;
            orderService.Client.User.LastName = viewModel.LastName;
            orderService.Client.User.Email = viewModel.Email;
            orderService.Client.User.PhoneNumber = viewModel.PhoneNumber;

            // Update the car information
            orderService.Car.Brand = viewModel.Brand;
            orderService.Car.Model = viewModel.Model;
            orderService.Car.Year = viewModel.Year;
            orderService.Car.Color = viewModel.Color;
            orderService.Car.VIN = viewModel.VIN;
            orderService.Car.Mileage = viewModel.Mileage;

            // Update the services (simplified)
            orderService.Services.Clear();
            var service = await _context.Services.FirstOrDefaultAsync(s => s.Name == viewModel.SelectedService);
            if (service != null)
            {
                orderService.Services.Add(service);
            }

            await _context.SaveChangesAsync();

            ViewBag.SuccessMessage = "Usługa została zaktualizowana pomyślnie.";
            viewModel.ServicesList = GetServicesList();
            return View("AddService", viewModel);
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

        [HttpGet]
        public async Task<IActionResult> Order()
        {
            var orderProducts = await _context.Orders
                .OfType<OrderProducts>()
                .Where(op => op.Status != "Zakończone")
                .Include(o => o.Client)
                    .ThenInclude(c => c.User)
                .Include(o => o.Products)
                .ToListAsync();

            var orderTotals = orderProducts.ToDictionary(
                order => order.OrderId,
                order => order.Products.Sum(p => p.Price)
            );

            var model = new OrderProductsViewModel
            {
                OrderProducts = orderProducts,
                OrderTotals = orderTotals,
                StatusList = new List<string> { "Nowe", "W realizacji", "Wysłane", "Zakończone" }
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderProductStatus(int id, string status)
        {
            var orderProduct = await _context.Orders.OfType<OrderProducts>().FirstOrDefaultAsync(o => o.OrderId == id);
            if (orderProduct == null)
            {
                return NotFound();
            }

            orderProduct.Status = status;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Order));
        }
    }
}
