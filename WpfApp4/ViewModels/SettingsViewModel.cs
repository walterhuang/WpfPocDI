using WpfApp4.Auth;

namespace WpfApp4.ViewModels
{
    [RequirePermission(Permissions.Settings)]
    public class SettingsViewModel : BaseViewModel
    {
        public string Title => "Settings";
        public string Message => "Configure your application settings here.";
    }
}
