using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace ProjektLABDetailing.Models
{
    public class Cart
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string SessionKey = "Cart";

        public Cart(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public List<CartItem> Items => GetCartItems();

        public decimal TotalPrice => Items.Sum(item => item.Price * item.Quantity);

        public List<CartItem> GetCartItems()
        {
            var session = _httpContextAccessor.HttpContext.Session;
            var cart = session.GetString(SessionKey);

            return cart == null ? new List<CartItem>() : JsonConvert.DeserializeObject<List<CartItem>>(cart);
        }

        public void AddToCart(CartItem item)
        {
            var cart = GetCartItems();
            var existingItem = cart.FirstOrDefault(c => c.ProductId == item.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                cart.Add(item);
            }

            SaveCart(cart);
        }

        public void UpdateQuantity(int productId, int quantity)
        {
            var cart = GetCartItems();
            var item = cart.FirstOrDefault(c => c.ProductId == productId);

            if (item != null)
            {
                item.Quantity = quantity;
            }

            SaveCart(cart);
        }

        public void RemoveFromCart(int productId)
        {
            var cart = GetCartItems();
            var item = cart.FirstOrDefault(c => c.ProductId == productId);

            if (item != null)
            {
                cart.Remove(item);
            }

            SaveCart(cart);
        }

        public void Clear()
        {
            SaveCart(new List<CartItem>());
        }

        private void SaveCart(List<CartItem> cart)
        {
            var session = _httpContextAccessor.HttpContext.Session;
            var cartJson = JsonConvert.SerializeObject(cart);
            session.SetString(SessionKey, cartJson);
        }
    }
}
