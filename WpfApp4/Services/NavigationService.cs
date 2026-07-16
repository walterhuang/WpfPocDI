using Microsoft.Extensions.DependencyInjection;
using WpfApp4.ViewModels;

namespace WpfApp4.Services
{
    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _serviceProvider;
        private MainViewModel? _mainViewModel;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        // Called once after MainViewModel is built to avoid a circular dependency
        public void Initialize(MainViewModel mainViewModel)
            => _mainViewModel = mainViewModel;

        public void NavigateTo<TViewModel>() where TViewModel : BaseViewModel
        {
            var viewModel = _serviceProvider.GetRequiredService<TViewModel>();
            _mainViewModel!.CurrentViewModel = viewModel;
        }
    }
}
