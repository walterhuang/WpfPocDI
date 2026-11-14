using System.Windows;
using System.Windows.Controls;
using WpfApp4.ViewModels;

namespace WpfApp4.Views
{
    public partial class LoginView : UserControl
    {
        public LoginView() => InitializeComponent();

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(UserNameBox.Text))
                UserNameBox.Focus();
            else
                PasswordBox.Focus();
        }

        // PasswordBox.Password is not bindable, so push it to the ViewModel manually
        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm)
                vm.Password = PasswordBox.Password;
        }

        // The TabControl may reuse this view for another tab's LoginViewModel (or recreate it),
        // so show the password that belongs to the current ViewModel
        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var password = (e.NewValue as LoginViewModel)?.Password ?? string.Empty;
            if (PasswordBox.Password != password)
                PasswordBox.Password = password;
        }
    }
}
