using System.Collections.ObjectModel;
using System.Windows.Input;
using WpfApp4.Services;

namespace WpfApp4.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly INavigationService _navigation;
        private TabItemViewModel? _selectedTab;

        public ObservableCollection<TabItemViewModel> Tabs { get; } = new();

        public TabItemViewModel? SelectedTab
        {
            get => _selectedTab;
            set => SetProperty(ref _selectedTab, value);
        }

        public ICommand NavigateHomeCommand { get; }
        public ICommand NavigateDashboardCommand { get; }
        public ICommand NavigateSettingsCommand { get; }
        public ICommand CloseTabCommand { get; }

        public MainViewModel(INavigationService navigation)
        {
            _navigation = navigation;

            // Let NavigationService know about us so it can open/activate tabs
            ((NavigationService)_navigation).Initialize(this);

            NavigateHomeCommand = new RelayCommand(() => _navigation.NavigateTo<HomeViewModel>("🏠  Home", isClosable: false));
            NavigateDashboardCommand = new RelayCommand(() => _navigation.NavigateTo<DashboardViewModel>("📊  Dashboard"));
            NavigateSettingsCommand = new RelayCommand(() => _navigation.NavigateTo<SettingsViewModel>("⚙️  Settings"));
            CloseTabCommand = new RelayCommand<TabItemViewModel>(CloseTab, tab => tab?.IsClosable == true);

            // Start on Home (pinned)
            _navigation.NavigateTo<HomeViewModel>("🏠  Home", isClosable: false);
        }

        public bool TryActivate(Type viewModelType)
        {
            var existing = Tabs.FirstOrDefault(t => t.ViewModelType == viewModelType);
            if (existing is null) return false;

            SelectedTab = existing;
            return true;
        }

        public void OpenTab(string header, BaseViewModel content, bool isClosable)
        {
            var tab = new TabItemViewModel(header, content, isClosable);
            Tabs.Add(tab);
            SelectedTab = tab;
        }

        private void CloseTab(TabItemViewModel? tab)
        {
            if (tab is null || !tab.IsClosable) return;

            var index = Tabs.IndexOf(tab);
            if (index < 0) return;

            var wasSelected = ReferenceEquals(tab, SelectedTab);
            Tabs.RemoveAt(index);

            if (wasSelected && Tabs.Count > 0)
                SelectedTab = Tabs[Math.Max(0, index - 1)];
        }
    }
}
