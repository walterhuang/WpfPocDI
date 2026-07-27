using System.Windows;
using WpfApp4.ViewModels;

namespace WpfApp4.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow(LoginViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            PasswordBox.PasswordChanged += (_, _) => viewModel.SetPassword(PasswordBox.Password);
            viewModel.LoginSucceeded += () => DialogResult = true;
        }
    }
}
