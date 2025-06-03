using Microsoft.Extensions.DependencyInjection;
using MotoStoreWpf.Services;
using System.Net.Http;
using System.Windows;

namespace MotoStoreWpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(new HttpClient());

            services.AddSingleton<ProductService>();
            services.AddSingleton<AuthService>();

            services.AddSingleton<ProductWindow>();
        }
    }
}
