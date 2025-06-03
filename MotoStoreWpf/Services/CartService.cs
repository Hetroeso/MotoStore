using MotoStore.Api.Models;

namespace MotoStoreWpf.Services
{
    public class CartService
    {
        private readonly List<Product> _cartItems = new();

        public void AddToCart(Product product)
        {
            _cartItems.Add(product);
        }

        public List<Product> GetCartItems()
        {
            return _cartItems;
        }

        public void ClearCart()
        {
            _cartItems.Clear();
        }
    }
}
