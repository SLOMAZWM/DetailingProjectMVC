using Microsoft.AspNetCore.Mvc;
using ProjektLABDetailing.Models;
using ProjektLABDetailing.Data;
using System.Threading.Tasks;

namespace ProjektLABDetailing.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly Cart _cart;

        public CartController(ApplicationDbContext context, Cart cart)
        {
            _context = context;
            _cart = cart;
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

        public IActionResult Checkout()
        {
            // Implement the checkout logic here
            return View();
        }
    }
}
