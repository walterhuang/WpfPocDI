using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using WpfApp4.Services;
using WpfApp4.ViewModels;
using WpfApp4.Views;

namespace WpfApp4
{
    public partial class App : Application
    {
        private IHost _host = null!;

        private async void OnStartup(object sender, StartupEventArgs e)
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // Services
                    services.AddSingleton<IGreetingService, GreetingService>();
                    services.AddSingleton<INavigationService, NavigationService>();
                    services.AddSingleton<IAuthService, AuthService>();

                    // ViewModels
                    services.AddSingleton<MainViewModel>();
                    services.AddTransient<HomeViewModel>();
                    services.AddTransient<DashboardViewModel>();
                    services.AddTransient<SettingsViewModel>();
                    services.AddTransient<LoginViewModel>();

                    // Shell
                    services.AddSingleton<MainWindow>();
                    services.AddTransient<LoginWindow>();
                })
                .Build();

            await _host.StartAsync();

            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            var loginWindow = _host.Services.GetRequiredService<LoginWindow>();
            if (loginWindow.ShowDialog() == true)
            {
                ShutdownMode = ShutdownMode.OnMainWindowClose;
                _host.Services.GetRequiredService<MainWindow>().Show();
            }
            else
            {
                Shutdown();
            }
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await _host.StopAsync();
            _host.Dispose();
            base.OnExit(e);
        }
    }
}
