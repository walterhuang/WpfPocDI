using System.Collections.ObjectModel;
using System.Windows.Input;
using WpfApp4.Auth;
using WpfApp4.Services;

namespace WpfApp4.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly INavigationService _navigation;
        private readonly IAuthService _auth;
        private TabItemViewModel? _selectedTab;

        public ObservableCollection<TabItemViewModel> Tabs { get; } = new();

        public TabItemViewModel? SelectedTab
        {
            get => _selectedTab;
            set => SetProperty(ref _selectedTab, value);
        }

        public bool IsAuthenticated => _auth.IsAuthenticated;

        public string AccountHeader => _auth.CurrentUser is { } user ? $"👤  {user.DisplayName}" : "👤  Guest";

        public string SignedInAsText => _auth.CurrentUser is { } user
            ? $"Signed in as {user.DisplayName} ({string.Join(", ", user.Roles)})"
            : "Not signed in";

        public ICommand NavigateHomeCommand { get; }
        public ICommand NavigateDashboardCommand { get; }
        public ICommand NavigateSettingsCommand { get; }
        public ICommand CloseTabCommand { get; }
        public ICommand LogoutCommand { get; }

        public MainViewModel(INavigationService navigation, IAuthService auth)
        {
            _navigation = navigation;
            _auth = auth;

            // Let NavigationService know about us so it can open/activate tabs
            ((NavigationService)_navigation).Initialize(this);

            NavigateHomeCommand = new RelayCommand(() => _navigation.NavigateTo<HomeViewModel>("🏠  Home", isClosable: false));
            NavigateDashboardCommand = new RelayCommand(() => _navigation.NavigateTo<DashboardViewModel>("📊  Dashboard"));
            NavigateSettingsCommand = new RelayCommand(() => _navigation.NavigateTo<SettingsViewModel>("⚙️  Settings"));
            CloseTabCommand = new RelayCommand<TabItemViewModel>(CloseTab, tab => tab?.IsClosable == true);
            LogoutCommand = new RelayCommand(_auth.Logout, () => _auth.IsAuthenticated);

            // Keep the Account menu in sync with sign-in / sign-out
            _auth.AuthStateChanged += OnAuthStateChanged;

            // Start on Home (pinned)
            _navigation.NavigateTo<HomeViewModel>("🏠  Home", isClosable: false);
        }

        public bool TryActivate(Type pageType)
        {
            var existing = Tabs.FirstOrDefault(t => t.PageType == pageType);
            if (existing is null) return false;

            SelectedTab = existing;
            return true;
        }

        public void OpenTab(TabItemViewModel tab)
        {
            Tabs.Add(tab);
            SelectedTab = tab;
        }

        public void RemoveTab(TabItemViewModel tab)
        {
            var index = Tabs.IndexOf(tab);
            if (index < 0) return;

            var wasSelected = ReferenceEquals(tab, SelectedTab);
            Tabs.RemoveAt(index);

            if (wasSelected && Tabs.Count > 0)
                SelectedTab = Tabs[Math.Max(0, index - 1)];
        }

        private void CloseTab(TabItemViewModel? tab)
        {
            if (tab is null || !tab.IsClosable) return;

            RemoveTab(tab);
        }

        private void OnAuthStateChanged(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(IsAuthenticated));
            OnPropertyChanged(nameof(AccountHeader));
            OnPropertyChanged(nameof(SignedInAsText));
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
