using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjektLABDetailing.Data;
using ProjektLABDetailing.Models;
using ProjektLABDetailing.Models.User;
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
                CartItems = _cart.Items.ToList()
            };
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    order.FirstName = user.FirstName;
                    order.LastName = user.LastName;
                }
            }
            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(OrderProduct order)
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    var user = await _userManager.GetUserAsync(User);
                    order.ClientId = int.TryParse(user.Id, out int userId) ? userId : 0;
                }
                else
                {
                    order.ClientId = GenerateGuestClientId();
                }

                order.OrderDate = DateTime.Now;
                order.Status = "Pending";
                order.TotalPrice = _cart.TotalPrice;

                foreach (var item in order.CartItems)
                {
                    order.Products.Add(new Product
                    {
                        ProductId = item.ProductId,
                        Name = item.ProductName,
                        Quantity = (short)item.Quantity,
                        Price = item.Price
                    });
                }

                _context.OrderProducts.Add(order);
                await _context.SaveChangesAsync();
                _cart.Clear();

                return RedirectToAction("OrderConfirmation", new { id = order.OrderId });
            }

            return View(order);
        }

        public async Task<IActionResult> OrderConfirmation(int id)
        {
            var order = await _context.OrderProducts
                .Include(o => o.Products)
                .FirstOrDefaultAsync(o => o.OrderId == id);
            return View(order);
        }

        private int GenerateGuestClientId()
        {
            int newId;
            do
            {
                newId = new Random().Next(-999999, -1);
            } while (_context.OrderProducts.Any(o => o.ClientId == newId));

            return newId;
        }
    }
}
