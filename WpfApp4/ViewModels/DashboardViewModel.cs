using WpfApp4.Auth;

namespace WpfApp4.ViewModels
{
    [RequirePermission(Permissions.Dashboard)]
    public class DashboardViewModel : BaseViewModel
    {
        public string Title => "Dashboard";
        public string Message => "Here is your Dashboard overview.";
    }
}
