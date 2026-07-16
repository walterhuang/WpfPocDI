using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Windows;
using WpfApp4.Services;
using WpfApp4.ViewModels;

namespace WpfApp4
{
    public partial class MainWindow : Window
    {
        private readonly ILogger<MainWindow> _logger;
        private readonly IConfiguration _configuration;
        private readonly IGreetingService _greetingService;

        public MainWindow(ILogger<MainWindow> logger, IConfiguration configuration,
                          IGreetingService greetingService, MainViewModel mainViewModel)
        {
            _logger = logger;
            _configuration = configuration;
            _greetingService = greetingService;

            InitializeComponent();

            DataContext = mainViewModel;

            _logger.LogInformation("MainWindow initialized.");
        }
    }
}
