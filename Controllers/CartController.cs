using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjektLABDetailing.Data;
using ProjektLABDetailing.Models;
using ProjektLABDetailing.Models.User;
using ProjektLABDetailing.Models.ViewModels;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjektLABDetailing.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly Cart _cart;
        private readonly UserManager<User> _userManager;

        public CartController(ApplicationDbContext context, Cart cart, UserManager<User> userManager)
        {
            _context = context;
            _cart = cart;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                var cartItem = new CartItem
                {
                    ProductId = product.ProductId,
                    ProductName = product.Name,
                    Quantity = 1,
                    Price = product.Price
                };

                _cart.AddToCart(cartItem);
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            _cart.UpdateQuantity(productId, quantity);
            return RedirectToAction("Summary");
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            _cart.RemoveFromCart(productId);
            return RedirectToAction("Summary");
        }

        public IActionResult Summary()
        {
            return View(_cart);
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            if (_cart.Items.Count == 0)
            {
                TempData["ErrorMessage"] = "Twój koszyk jest pusty. Nie można złożyć zamówienia.";
                return RedirectToAction("Summary");
            }

            var order = new OrderProduct
            {
                TotalPrice = _cart.TotalPrice,
                CartItems = _cart.Items.ToList()
            };
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    order.FirstName = user.FirstName;
                    order.LastName = user.LastName;
                    order.Email = user.Email;
                    order.PhoneNumber = user.PhoneNumber;
                }
            }
            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(OrderProduct order)
        {
            var validationErrors = ValidateOrder(order);
            if (validationErrors.Any())
            {
                foreach (var error in validationErrors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                order.CartItems = _cart.Items.ToList();
                return View(order);
            }

            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == user.Id);
                if (client == null)
                {
                    client = new Client
                    {
                        UserId = user.Id,
                        User = user,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        PhoneNumber = user.PhoneNumber
                    };
                    _context.Clients.Add(client);
                    await _context.SaveChangesAsync();
                }
                order.ClientId = client.ClientId;
            }
            else
            {
                var client = new Client
                {
                    User = new User
                    {
                        FirstName = order.FirstName,
                        LastName = order.LastName,
                        Email = order.Email,
                        PhoneNumber = order.PhoneNumber,
                        UserName = order.Email,
                        NormalizedUserName = order.Email.ToUpper(),
                        NormalizedEmail = order.Email.ToUpper()
                    },
                    Email = order.Email,
                    FirstName = order.FirstName,
                    LastName = order.LastName,
                    PhoneNumber = order.PhoneNumber
                };
                _context.Clients.Add(client);
                await _context.SaveChangesAsync();
                order.ClientId = client.ClientId;
            }

            order.OrderDate = DateTime.Now;
            order.Status = "Pending";
            order.TotalPrice = _cart.TotalPrice;

            foreach (var item in _cart.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    var orderProductDetail = new ProjektLABDetailing.Models.OrderProductDetail
                    {
                        ProductId = product.ProductId,
                        Product = product,
                        Quantity = item.Quantity,
                        Price = item.Price
                    };
                    order.OrderProductDetails.Add(orderProductDetail);
                }
            }

            _context.OrderProducts.Add(order);
            await _context.SaveChangesAsync();
            _cart.Clear();

            return RedirectToAction("OrderConfirmation", new { id = order.OrderId });
        }

        private List<string> ValidateOrder(OrderProduct order)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(order.FirstName))
            {
                errors.Add("Imię jest wymagane.");
            }
            if (string.IsNullOrWhiteSpace(order.LastName))
            {
                errors.Add("Nazwisko jest wymagane.");
            }
            if (string.IsNullOrWhiteSpace(order.Email))
            {
                errors.Add("Adres e-mail jest wymagany.");
            }
            else
            {
                if (!new EmailAddressAttribute().IsValid(order.Email))
                {
                    errors.Add("Nieprawidłowy format adresu e-mail.");
                }
                else if (order.Email.Count(c => c == '@') != 1)
                {
                    errors.Add("Adres e-mail musi zawierać dokładnie jeden znak '@'.");
                }
            }
            if (string.IsNullOrWhiteSpace(order.PhoneNumber) || !new PhoneAttribute().IsValid(order.PhoneNumber))
            {
                errors.Add("Nieprawidłowy format numeru telefonu.");
            }
            if (string.IsNullOrWhiteSpace(order.Address))
            {
                errors.Add("Adres jest wymagany.");
            }
            if (string.IsNullOrWhiteSpace(order.City))
            {
                errors.Add("Miasto jest wymagane.");
            }
            if (string.IsNullOrWhiteSpace(order.PostalCode))
            {
                errors.Add("Kod pocztowy jest wymagany.");
            }
            if (string.IsNullOrWhiteSpace(order.PaymentMethod))
            {
                errors.Add("Metoda płatności jest wymagana.");
            }
            if (string.IsNullOrWhiteSpace(order.DeliveryMethod))
            {
                errors.Add("Sposób dostawy jest wymagany.");
            }

            return errors;
        }

        [HttpGet]
        public async Task<IActionResult> OrderConfirmation(int id)
        {
            var order = await _context.OrderProducts
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
                ClientName = $"{order.FirstName} {order.LastName}",
                ClientEmail = order.Email,
                ClientPhoneNumber = order.PhoneNumber,
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
