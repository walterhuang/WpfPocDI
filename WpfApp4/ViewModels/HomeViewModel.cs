using WpfApp4.Auth;

namespace WpfApp4.ViewModels
{
    [AllowAnonymous]
    public class HomeViewModel : BaseViewModel
    {
        public string Title => "Home";
        public string Message => "Welcome to the Home page!";
    }
}
