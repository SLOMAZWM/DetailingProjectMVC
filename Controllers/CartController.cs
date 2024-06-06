using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjektLABDetailing.Data;
using ProjektLABDetailing.Models;
using ProjektLABDetailing.Models.User;
using ProjektLABDetailing.Models.ViewModels;
using System;
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
