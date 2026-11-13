using System.Windows.Input;

namespace WpfApp4.ViewModels
{
    // Shown inside a tab when the signed-in user lacks the access right for the requested page
    public class AccessDeniedViewModel : BaseViewModel
    {
        private string _message = "You do not have permission to open this page.";

        public AccessDeniedViewModel()
        {
            SwitchUserCommand = new RelayCommand(() => SwitchUserRequested?.Invoke(this, EventArgs.Empty));
        }

        public string Title => "⛔  Access Denied";

        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public ICommand SwitchUserCommand { get; }

        public event EventHandler? SwitchUserRequested;
    }
}
