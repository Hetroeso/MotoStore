using MotoStoreWpf.Services;
using System.Windows;
using System.Windows.Controls;

namespace MotoStoreWpf
{
    public partial class LoginWindow : Window
    {
        private readonly AuthService _authService;

        public LoginWindow()
        {
            InitializeComponent();
            _authService = new AuthService(new System.Net.Http.HttpClient());

            // Заглушка — автоматическая авторизация
            //AutoLogin();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var login = UsernameTextBox.Text;
            var password = PasswordBox.Password;

            var response = await _authService.LoginAsync(login, password);

            if (response is true)
            {
                MessageBox.Show("Успешный вход");
                var productWindow = new ProductWindow(_authService);
                productWindow.Show();

                this.Close();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль");
            }
        }

        //private async void AutoLogin()
        //{
        //    try
        //    {
        //        // Вход под тестовым админом
        //        var token = await _authService.LoginAsync("SuperAdmin", "1234");

        //        if (token)
        //        {
        //            // Можно сохранить токен, если нужно
        //            App.Current.Properties["AuthToken"] = token;

        //            // Переход в главное окно
        //            var productWindow = new ProductWindow(_authService);
        //            productWindow.Show();
        //            Close();
        //        }
        //        else
        //        {
        //            MessageBox.Show("Автовход не удался.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Ошибка входа: " + ex.Message);
        //    }
        //}
    }
}
