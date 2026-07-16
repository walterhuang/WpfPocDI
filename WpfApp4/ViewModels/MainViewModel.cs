using System.Windows.Input;
using WpfApp4.Services;

namespace WpfApp4.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly INavigationService _navigation;
        private BaseViewModel? _currentViewModel;

        public BaseViewModel? CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
        }

        public ICommand NavigateHomeCommand { get; }
        public ICommand NavigateDashboardCommand { get; }
        public ICommand NavigateSettingsCommand { get; }

        public MainViewModel(INavigationService navigation)
        {
            _navigation = navigation;

            // Let NavigationService know about us so it can set CurrentViewModel
            ((NavigationService)_navigation).Initialize(this);

            NavigateHomeCommand      = new RelayCommand(() => _navigation.NavigateTo<HomeViewModel>());
            NavigateDashboardCommand = new RelayCommand(() => _navigation.NavigateTo<DashboardViewModel>());
            NavigateSettingsCommand  = new RelayCommand(() => _navigation.NavigateTo<SettingsViewModel>());

            // Start on Home
            _navigation.NavigateTo<HomeViewModel>();
        }
    }
}
