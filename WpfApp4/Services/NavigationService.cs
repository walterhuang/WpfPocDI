using Microsoft.Extensions.DependencyInjection;
using WpfApp4.Auth;
using WpfApp4.ViewModels;

namespace WpfApp4.Services
{
    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IAuthService _auth;
        private MainViewModel? _mainViewModel;

        public NavigationService(IServiceProvider serviceProvider, IAuthService auth)
        {
            _serviceProvider = serviceProvider;
            _auth = auth;

            // Re-evaluate open tabs whenever the user signs in, switches user or signs out
            _auth.AuthStateChanged += OnAuthStateChanged;
        }

        // Called once after MainViewModel is built to avoid a circular dependency
        public void Initialize(MainViewModel mainViewModel)
            => _mainViewModel = mainViewModel;

        public void NavigateTo<TViewModel>(string header, bool isClosable = true) where TViewModel : BaseViewModel
        {
            // Switch to the tab if it is already open; otherwise open a new one
            if (_mainViewModel!.TryActivate(typeof(TViewModel))) return;

            var tab = new TabItemViewModel(header, typeof(TViewModel), isClosable);
            UpdateTabContent(tab);
            _mainViewModel.OpenTab(tab);
        }

        // Shows the requested page, or the login form / Access Denied view in its place
        private void UpdateTabContent(TabItemViewModel tab)
        {
            switch (_auth.CheckAccess(tab.PageType))
            {
                case PageAccess.Allowed:
                    // Keep an already-open page so its state survives a user switch
                    if (tab.Content?.GetType() != tab.PageType)
                        tab.Content = (BaseViewModel)_serviceProvider.GetRequiredService(tab.PageType);
                    break;

                case PageAccess.LoginRequired:
                    if (tab.Content is not LoginViewModel)
                        tab.Content = CreateLogin($"Please sign in to open {tab.Header}.");
                    break;

                case PageAccess.Forbidden:
                    // Always recreate so the message reflects the current user
                    tab.Content = CreateAccessDenied(tab);
                    break;
            }
        }

        private LoginViewModel CreateLogin(string prompt)
        {
            var login = _serviceProvider.GetRequiredService<LoginViewModel>();
            login.Prompt = prompt;
            return login;
        }

        private AccessDeniedViewModel CreateAccessDenied(TabItemViewModel tab)
        {
            var denied = _serviceProvider.GetRequiredService<AccessDeniedViewModel>();
            denied.Message = $"{_auth.CurrentUser?.DisplayName} does not have permission to open {tab.Header}.";

            // Keep the current session until the new sign-in succeeds
            denied.SwitchUserRequested += (_, _) =>
                tab.Content = CreateLogin($"Sign in as a different user to open {tab.Header}.");

            return denied;
        }

        private void OnAuthStateChanged(object? sender, EventArgs e)
        {
            if (_mainViewModel is null) return;

            foreach (var tab in _mainViewModel.Tabs.ToList())
            {
                // Signed out: close every tab that is not available anonymously
                if (!_auth.IsAuthenticated && _auth.CheckAccess(tab.PageType) != PageAccess.Allowed)
                    _mainViewModel.RemoveTab(tab);
                else
                    UpdateTabContent(tab);
            }

            if (!_auth.IsAuthenticated)
                _mainViewModel.SelectedTab = _mainViewModel.Tabs.FirstOrDefault();
        }
    }
}
