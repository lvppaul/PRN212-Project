using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;

namespace Project_KoiCareSystemAtHome
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider serviceProvider;

        public App()
        {
            ServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            serviceProvider = services.BuildServiceProvider();
        }
        private void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton<Login>();
            services.AddHttpClient();
        }

        private void OnStartup (object render, StartupEventArgs args)
        {
            var login = serviceProvider.GetRequiredService<Login>();
            login.Show();
        }
    }

}
