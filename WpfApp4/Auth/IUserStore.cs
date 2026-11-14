namespace WpfApp4.Auth
{
    // Validates user credentials. POC implementation: ConfigurationUserStore (appsettings.json).
    // Replace with a Web API / SQL implementation and update the DI registration in App.xaml.cs.
    public interface IUserStore
    {
        Task<AppUser?> ValidateCredentialsAsync(string userName, string password, CancellationToken cancellationToken = default);
    }
}
