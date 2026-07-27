using System.Windows.Input;
using WpfApp4.Services;

namespace WpfApp4.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;

        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _errorMessage = string.Empty;

        public event Action? LoginSucceeded;

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand LoginCommand { get; }

        public LoginViewModel(IAuthService authService)
        {
            _authService = authService;
            LoginCommand = new RelayCommand(ExecuteLogin, () => !string.IsNullOrWhiteSpace(Username));
        }

        public void SetPassword(string password)
            => _password = password;

        private void ExecuteLogin()
        {
            if (_authService.Validate(Username, _password))
            {
                ErrorMessage = string.Empty;
                LoginSucceeded?.Invoke();
            }
            else
            {
                ErrorMessage = "Invalid username or password.";
            }
        }
    }
}
