using Microsoft.AspNetCore.Mvc;
using ProjektLABDetailing.Models;
using ProjektLABDetailing.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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

        public IActionResult Index()
        {
            return View(_cart);
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

        public IActionResult Summary()
        {
            return PartialView("_CartSummary", _cart);
        }
    }
}
