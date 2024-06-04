using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjektLABDetailing.Data;
using ProjektLABDetailing.Models;
using ProjektLABDetailing.Models.ViewModels;
using ProjektLABDetailing.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

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
            var orderProducts = await _context.OrderProducts
                .Where(op => op.Status == "Zakończone")
                .Include(o => o.Client)
                    .ThenInclude(c => c.User)
                .Include(o => o.OrderProductDetails)
                    .ThenInclude(opd => opd.Product)
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
                order => order.OrderProductDetails.Sum(opd => opd.Price * opd.Quantity)
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

            var orderServiceViewModels = orderServices.Select(os => new OrderServiceViewModel
            {
                OrderId = os.OrderId,
                ClientName = os.Client?.User != null ? GetFullName(os.Client.User) : "Unknown",
                CarDetails = os.Car != null ? $"{os.Car.Brand} {os.Car.Model}" : "Unknown",
                ServiceName = os.Services.FirstOrDefault()?.Name ?? "Unknown",
                Status = os.Status ?? "Unknown",
                ExecutionDate = os.ExecutionDate ?? DateTime.MinValue,
                Materials = os.Materials ?? "Brak",
                Condition = os.Condition ?? "Brak",
                ClientRemarks = os.ClientRemarks ?? "Brak"
            }).ToList();

            var model = new ServicesViewModel
            {
                OrderServices = orderServiceViewModels,
                StatusList = new List<string> { "Oczekuje", "W trakcie", "Zrealizowane", "Zakończone" }
            };

            return View(model);
        }

        public string GetFullName(User user)
        {
            return $"{user.FirstName} {user.LastName}";
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var orderService = await _context.OrderServices.FindAsync(id);
                    if (orderService == null)
                    {
                        return NotFound();
                    }

                    orderService.Status = status;
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return RedirectToAction(nameof(Services));
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError("", "Wystąpił błąd podczas aktualizacji statusu.");
                    return RedirectToAction(nameof(Services));
                }
            }
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

            var errors = ValidateAddServiceViewModel(viewModel);
            if (errors.Count > 0)
            {
                ViewBag.Errors = errors;
                return View(viewModel);
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    User user = await _context.Users.SingleOrDefaultAsync(u => u.Email == viewModel.Email);
                    if (user == null)
                    {
                        user = new User
                        {
                            FirstName = viewModel.FirstName,
                            LastName = viewModel.LastName,
                            Email = viewModel.Email,
                            PhoneNumber = viewModel.PhoneNumber,
                            UserName = viewModel.Email
                        };

                        var result = await _signInManager.UserManager.CreateAsync(user, "DefaultPassword@123");
                        if (!result.Succeeded)
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                            return View(viewModel);
                        }
                    }

                    Client client = await _context.Clients.SingleOrDefaultAsync(c => c.UserId == user.Id);
                    if (client == null)
                    {
                        client = new Client
                        {
                            UserId = user.Id,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Email = user.Email,
                            PhoneNumber = user.PhoneNumber,
                            User = user
                        };

                        _context.Clients.Add(client);
                        await _context.SaveChangesAsync();
                    }

                    var car = new Car
                    {
                        ClientId = client.ClientId,
                        Brand = viewModel.Brand,
                        Model = viewModel.Model,
                        Year = viewModel.Year,
                        Color = viewModel.Color,
                        VIN = viewModel.VIN,
                        Mileage = viewModel.Mileage,
                        Condition = viewModel.Condition
                    };

                    _context.Cars.Add(car);
                    await _context.SaveChangesAsync();

                    var orderService = new OrderService
                    {
                        ClientId = client.ClientId,
                        CarId = car.CarId,
                        ExecutionDate = viewModel.ExecutionDate,
                        Status = "Oczekuje",
                        Materials = viewModel.Materials,
                        Condition = viewModel.Condition,
                        ClientRemarks = viewModel.ClientRemarks,
                        Services = new List<Service> { _context.Services.Find(viewModel.SelectedServiceId) }
                    };

                    _context.OrderServices.Add(orderService);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    TempData["SuccessMessage"] = "Usługa została dodana pomyślnie.";
                    return RedirectToAction(nameof(Services));
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError("", "Wystąpił błąd podczas dodawania usługi.");
                    return View(viewModel);
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditService(int selectedOrderId)
        {
            var orderService = await _context.OrderServices
                .Include(os => os.Client)
                    .ThenInclude(c => c.User)
                .Include(os => os.Car)
                .Include(os => os.Services)
                .FirstOrDefaultAsync(os => os.OrderId == selectedOrderId);

            if (orderService == null)
            {
                return NotFound();
            }

            var model = new EditServiceViewModel
            {
                OrderId = orderService.OrderId,
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
                Condition = orderService.Car.Condition,
                ExecutionDate = orderService.ExecutionDate ?? DateTime.MinValue,
                SelectedServiceId = orderService.Services.FirstOrDefault()?.ServiceId ?? 0,
                Materials = orderService.Materials,
                ClientRemarks = orderService.ClientRemarks,
                ServicesList = GetServicesList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditService(EditServiceViewModel viewModel)
        {
            viewModel.ServicesList = GetServicesList();

            var errors = ValidateEditServiceViewModel(viewModel);
            if (errors.Count > 0)
            {
                ViewBag.Errors = errors;
                return View(viewModel);
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
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
                    orderService.Condition = viewModel.Condition;
                    orderService.ClientRemarks = viewModel.ClientRemarks;

                    orderService.Client.User.FirstName = viewModel.FirstName;
                    orderService.Client.User.LastName = viewModel.LastName;
                    orderService.Client.User.Email = viewModel.Email;
                    orderService.Client.User.PhoneNumber = viewModel.PhoneNumber;

                    orderService.Car.Brand = viewModel.Brand;
                    orderService.Car.Model = viewModel.Model;
                    orderService.Car.Year = viewModel.Year;
                    orderService.Car.Color = viewModel.Color;
                    orderService.Car.VIN = viewModel.VIN;
                    orderService.Car.Mileage = viewModel.Mileage;

                    orderService.Services = new List<Service> { _context.Services.Find(viewModel.SelectedServiceId) };

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    TempData["SuccessMessage"] = "Usługa została zaktualizowana pomyślnie.";
                    return RedirectToAction(nameof(Services));
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError("", "Wystąpił błąd podczas edycji usługi.");
                    return View(viewModel);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteService(int id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var orderService = await _context.OrderServices.FindAsync(id);
                    if (orderService == null)
                    {
                        return NotFound();
                    }

                    _context.OrderServices.Remove(orderService);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return RedirectToAction(nameof(Services));
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError("", "Wystąpił błąd podczas usuwania usługi.");
                    return RedirectToAction(nameof(Services));
                }
            }
        }

        private List<SelectListItem> GetServicesList()
        {
            return _context.Services
                .Select(s => new SelectListItem
                {
                    Value = s.ServiceId.ToString(),
                    Text = s.Name
                })
                .ToList();
        }

        [HttpGet]
        public async Task<IActionResult> Order()
        {
            var orderProducts = await _context.OrderProducts
                .Where(op => op.Status != "Zakończone")
                .Include(o => o.Client)
                    .ThenInclude(c => c.User)
                .Include(o => o.OrderProductDetails)
                    .ThenInclude(opd => opd.Product)
                .ToListAsync();

            var orderTotals = orderProducts.ToDictionary(
                order => order.OrderId,
                order => order.OrderProductDetails.Sum(opd => opd.Price * opd.Quantity)
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
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var orderProduct = await _context.OrderProducts.FirstOrDefaultAsync(o => o.OrderId == id);
                    if (orderProduct == null)
                    {
                        return NotFound();
                    }

                    orderProduct.Status = status;
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return RedirectToAction(nameof(Order));
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError("", "Wystąpił błąd podczas aktualizacji statusu zamówienia produktu.");
                    return RedirectToAction(nameof(Order));
                }
            }
        }

        private List<string> ValidateAddServiceViewModel(AddServiceViewModel model)
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(model.FirstName))
                errors.Add("Imię jest wymagane.");

            if (string.IsNullOrEmpty(model.LastName))
                errors.Add("Nazwisko jest wymagane.");

            if (string.IsNullOrEmpty(model.Email) || !new EmailAddressAttribute().IsValid(model.Email))
                errors.Add("Nieprawidłowy format email.");

            if (string.IsNullOrEmpty(model.PhoneNumber) || !Regex.IsMatch(model.PhoneNumber, @"^\+\d{1,3}\s?\d{1,3}\s?\d{3}\s?\d{3}\s?\d{3}$"))
                errors.Add("Numer telefonu musi być w formacie +00 000 000 000.");

            if (string.IsNullOrEmpty(model.Brand))
                errors.Add("Marka jest wymagana.");

            if (string.IsNullOrEmpty(model.Model))
                errors.Add("Model jest wymagany.");

            if (model.Year <= 0)
                errors.Add("Rok jest wymagany.");

            if (string.IsNullOrEmpty(model.Color))
                errors.Add("Kolor jest wymagany.");

            if (string.IsNullOrEmpty(model.VIN))
                errors.Add("VIN jest wymagany.");

            if (model.Mileage <= 0)
                errors.Add("Przebieg jest wymagany.");

            if (model.ExecutionDate == DateTime.MinValue)
                errors.Add("Data realizacji jest wymagana.");

            if (model.SelectedServiceId <= 0)
                errors.Add("Wybór usługi jest wymagany.");

            if (string.IsNullOrEmpty(model.Materials))
                errors.Add("Materiały są wymagane.");

            if (string.IsNullOrEmpty(model.ClientRemarks))
                errors.Add("Uwagi dla klienta są wymagane.");

            return errors;
        }

        private List<string> ValidateEditServiceViewModel(EditServiceViewModel model)
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(model.FirstName))
                errors.Add("Imię jest wymagane.");

            if (string.IsNullOrEmpty(model.LastName))
                errors.Add("Nazwisko jest wymagane.");

            if (string.IsNullOrEmpty(model.Email) || !new EmailAddressAttribute().IsValid(model.Email))
                errors.Add("Nieprawidłowy format email.");

            if (string.IsNullOrEmpty(model.PhoneNumber) || !Regex.IsMatch(model.PhoneNumber, @"^\+\d{1,3}\s?\d{1,3}\s?\d{3}\s?\d{3}\s?\d{3}$"))
                errors.Add("Numer telefonu musi być w formacie +00 000 000 000.");

            if (string.IsNullOrEmpty(model.Brand))
                errors.Add("Marka jest wymagana.");

            if (string.IsNullOrEmpty(model.Model))
                errors.Add("Model jest wymagany.");

            if (model.Year <= 0)
                errors.Add("Rok jest wymagany.");

            if (string.IsNullOrEmpty(model.Color))
                errors.Add("Kolor jest wymagany.");

            if (string.IsNullOrEmpty(model.VIN))
                errors.Add("VIN jest wymagany.");

            if (model.Mileage <= 0)
                errors.Add("Przebieg jest wymagany.");

            if (model.ExecutionDate == DateTime.MinValue)
                errors.Add("Data realizacji jest wymagana.");

            if (model.SelectedServiceId <= 0)
                errors.Add("Wybór usługi jest wymagany.");

            if (string.IsNullOrEmpty(model.Materials))
                errors.Add("Materiały są wymagane.");

            if (string.IsNullOrEmpty(model.ClientRemarks))
                errors.Add("Uwagi dla klienta są wymagane.");

            return errors;
        }

        [HttpGet]
        public async Task<IActionResult> OrderDetails(int id)
        {
            var order = await _context.OrderProducts
                .Include(o => o.Client)
                    .ThenInclude(c => c.User)
                .Include(o => o.OrderProductDetails)
                    .ThenInclude(opd => opd.Product)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            var orderDetailsViewModel = new OrderDetailsViewModel
            {
                OrderId = order.OrderId,
                ClientName = $"{order.Client.User.FirstName} {order.Client.User.LastName}",
                ClientEmail = order.Client.User.Email,
                ClientPhoneNumber = order.Client.User.PhoneNumber,
                Address = order.Address,
                City = order.City,
                PostalCode = order.PostalCode,
                PaymentMethod = order.PaymentMethod,
                DeliveryMethod = order.DeliveryMethod,
                OrderDate = order.OrderDate,
                TotalPrice = order.TotalPrice,
                Status = order.Status,
                Products = order.OrderProductDetails.Select(opd => new ProjektLABDetailing.Models.ViewModels.OrderProductDetail
                {
                    ProductId = opd.ProductId,
                    Name = opd.Product.Name,
                    Quantity = opd.Quantity,
                    Price = opd.Price
                }).ToList()
            };

            return View(orderDetailsViewModel);
        }
    }
}
