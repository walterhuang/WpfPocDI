using System.Windows.Input;
using WpfApp4.Auth;

namespace WpfApp4.ViewModels
{
    // Shown inside a tab whose page requires sign-in. It only signs the user in; IAuthService.AuthStateChanged
    // lets NavigationService swap the tab to the requested page (or Access Denied).
    public class LoginViewModel : BaseViewModel
    {
        private readonly IAuthService _auth;
        private string _prompt = "Please sign in to continue.";
        private string _userName = string.Empty;
        private string _password = string.Empty;
        private string? _errorMessage;
        private bool _isBusy;

        public LoginViewModel(IAuthService auth)
        {
            _auth = auth;
            LoginCommand = new AsyncRelayCommand(LoginAsync, CanLogin);
        }

        public string Prompt
        {
            get => _prompt;
            set => SetProperty(ref _prompt, value);
        }

        public string UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }

        // Pushed from LoginView code-behind because PasswordBox.Password is not bindable
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string? ErrorMessage
        {
            get => _errorMessage;
            private set
            {
                if (SetProperty(ref _errorMessage, value))
                    OnPropertyChanged(nameof(HasError));
            }
        }

        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

        public bool IsBusy
        {
            get => _isBusy;
            private set => SetProperty(ref _isBusy, value);
        }

        public ICommand LoginCommand { get; }

        private bool CanLogin() => !IsBusy && !string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrEmpty(Password);

        private async Task LoginAsync()
        {
            ErrorMessage = null;
            IsBusy = true;
            try
            {
                if (!await _auth.LoginAsync(UserName.Trim(), Password))
                    ErrorMessage = "Invalid user name or password.";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Sign-in failed: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
