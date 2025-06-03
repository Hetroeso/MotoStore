using MotoStore.Api.Models;
using MotoStoreWpf.Services;
using System.Windows;

namespace MotoStoreWpf
{
    public partial class ProductEditWindow : Window
    {
        private readonly ProductService _productService;
        private Product _product;

        public ProductEditWindow(ProductService productService, Product existingProduct)
        {
            InitializeComponent();
            _productService = productService;
            _product = existingProduct ?? new Product();

            NameTextBox.Text = _product.Name;
            PriceTextBox.Text = _product.Price.ToString("0.00");
            CategoryTextBox.Text = _product.Category;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _product.Name = NameTextBox.Text;
            _product.Category = CategoryTextBox.Text;

            if (decimal.TryParse(PriceTextBox.Text, out var price))
            {
                _product.Price = price;
            }
            else
            {
                MessageBox.Show("Введите корректную цену", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool success = await _productService.AddProductAsync(_product);

            if (success)
            {
                MessageBox.Show("Товар успешно обновлён", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Ошибка при сохранении товара", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
