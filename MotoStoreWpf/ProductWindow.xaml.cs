using MotoStore.Api.Models;
using MotoStoreWpf.Services;
using System.Windows;

namespace MotoStoreWpf
{
    public partial class ProductWindow : Window
    {
        private readonly ProductService _productService;
        private readonly OrderService _orderService;
        private readonly CartService _cartService;
        private readonly AuthService _authService;
        private Product _product;

        public ProductWindow(AuthService authService)
        {
            InitializeComponent();
            _productService = new ProductService(new System.Net.Http.HttpClient());
            _orderService = new OrderService(new System.Net.Http.HttpClient());
            _cartService = new CartService();
            _authService = authService;
            _product = new Product();
        LoadProducts();
        }

        private async void LoadProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            ProductListView.ItemsSource = products;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadProducts();
        }

        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProductListView.SelectedItem is Product selectedProduct)
            {
                // Создаем копию, чтобы избежать изменений в списке до подтверждения
                var productToEdit = new Product
                {
                    Id = selectedProduct.Id,
                    Name = selectedProduct.Name,
                    Category = selectedProduct.Category,
                    Price = selectedProduct.Price
                };

                var editWindow = new ProductEditWindow(_productService, productToEdit);
                var result = editWindow.ShowDialog();

                if (result == true)
                {
                    try
                    {
                        await _productService.UpdateProductAsync(productToEdit);
                        MessageBox.Show("Товар успешно обновлён.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadProducts();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при обновлении товара: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите товар для редактирования.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void BuyButton_Click(object sender, RoutedEventArgs e)
        {
            var cartItems = _cartService.GetCartItems();
            if (cartItems.Count == 0)
            {
                MessageBox.Show("Корзина пуста.");
                return;
            }

            var order = new Order
            {
                CustomerName = _authService.CurrentUserName ?? "Неизвестный пользователь",
                OrderDate = DateTime.Now,
                TotalAmount = cartItems.Sum(p => p.Price),
                Items = cartItems.Select(p => new OrderItem
                {
                    ProductId = p.Id,
                    Quantity = 1,
                    UnitPrice = p.Price
                }).ToList()
            };

            try
            {
                await _orderService.CreateOrderAsync(order);
                MessageBox.Show("Заказ успешно оформлен.");
                _cartService.ClearCart();
                RefreshCart();
                UpdateTotalAmount();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при оформлении заказа: " + ex.Message);
            }
        }

        private void RefreshCart()
        {
            CartListView.ItemsSource = null;
            CartListView.ItemsSource = _cartService.GetCartItems();
        }

        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            if (ProductListView.SelectedItem is Product selectedProduct)
            {
                _cartService.AddToCart(selectedProduct);
                UpdateTotalAmount();
                RefreshCart();
            }
        }

        private void UpdateTotalAmount()
        {
            var total = _cartService.GetCartItems().Sum(p => p.Price);
            TotalAmountTextBlock.Text = $"{total:C}";
        }

        private void ClearCartButton_Click(object sender, RoutedEventArgs e)
        {
            _cartService.ClearCart();
            RefreshCart();
        }
        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new ProductEditWindow(_productService, _product); // новое или пустое окно
            editWindow.ShowDialog();
            LoadProducts(); // перезагрузка списка
        }

        private async void DeleteProductButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedProduct = ProductListView.SelectedItem as Product;
            if (selectedProduct == null)
            {
                MessageBox.Show("Пожалуйста, выберите товар для удаления.");
                return;
            }

            var result = MessageBox.Show($"Вы уверены, что хотите удалить товар \"{selectedProduct.Name}\"?",
                                         "Подтверждение удаления",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    bool success = await _productService.DeleteProductAsync(selectedProduct.Id);
                    if (success)
                    {
                        MessageBox.Show("Товар удалён.");
                        LoadProducts();
                    }
                    else
                    {
                        MessageBox.Show("Не удалось удалить товар.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении товара: {ex.Message}");
                }
            }
        }
    }
}