using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using ProjektLABDetailing.Models.User;
using Microsoft.AspNetCore.Authorization;
using ProjektLABDetailing.Data;
using Microsoft.EntityFrameworkCore;
using ProjektLABDetailing.Models.ViewModels;
using System.Linq;

namespace ProjektLABDetailing.Controllers
{
    [Authorize]
    public class ClientController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext _context;

        public ClientController(SignInManager<User> signInManager, ApplicationDbContext context)
        {
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult ClientUserPanel()
        {
            var userType = HttpContext.Session.GetString("UserType");
            if (userType != "Client")
            {
                return RedirectToAction("Login", "Account");
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
        public IActionResult ChangeDataUser()
        {
            return RedirectToAction("ChangeDataUser", "Account");
        }

        [HttpGet]
        public async Task<IActionResult> ServiceHistory()
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var services = await _context.OrderServices
                .Include(os => os.Car)
                .Include(os => os.Services)
                .Where(os => os.Client.UserId == user.Id)
                .ToListAsync();

            return View(services);
        }

        [HttpGet]
        public async Task<IActionResult> OrderHistory()
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var orders = await _context.OrderProducts
                .Where(op => op.Client.UserId == user.Id)
                .Include(op => op.OrderProductDetails)
                    .ThenInclude(opd => opd.Product)
                .ToListAsync();

            return View(orders);
        }

        [HttpGet]
        public async Task<IActionResult> OrderDetails(int id)
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var order = await _context.OrderProducts
                .Include(o => o.Client)
                    .ThenInclude(c => c.User)
                .Include(o => o.OrderProductDetails)
                    .ThenInclude(opd => opd.Product)
                .SingleOrDefaultAsync(o => o.OrderId == id && o.Client.UserId == user.Id);

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
                Products = order.OrderProductDetails.Select(opd => new OrderProductDetail
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
