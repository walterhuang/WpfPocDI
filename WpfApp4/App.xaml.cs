using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using WpfApp4.Auth;
using WpfApp4.Services;
using WpfApp4.ViewModels;

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

                    // Auth (POC: users / roles / access rights come from appsettings.json).
                    // Swap the two stores for Web API / SQL implementations later.
                    services.Configure<AuthOptions>(context.Configuration.GetSection(AuthOptions.SectionName));
                    services.AddSingleton<IUserStore, ConfigurationUserStore>();
                    services.AddSingleton<IAccessRightsStore, ConfigurationAccessRightsStore>();
                    services.AddSingleton<IAuthService, AuthService>();

                    // ViewModels
                    services.AddSingleton<MainViewModel>();
                    services.AddTransient<HomeViewModel>();
                    services.AddTransient<DashboardViewModel>();
                    services.AddTransient<SettingsViewModel>();
                    services.AddTransient<LoginViewModel>();
                    services.AddTransient<AccessDeniedViewModel>();

                    // Shell
                    services.AddSingleton<MainWindow>();
                })
                .Build();

            await _host.StartAsync();

            _host.Services.GetRequiredService<MainWindow>().Show();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await _host.StopAsync();
            _host.Dispose();
            base.OnExit(e);
        }
    }
}
