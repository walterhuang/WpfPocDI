namespace WpfApp4.Services
{
    public class AuthService : IAuthService
    {
        private const string ValidUsername = "admin";
        private const string ValidPassword = "password";

        public bool Validate(string username, string password)
            => username == ValidUsername && password == ValidPassword;
    }
}
