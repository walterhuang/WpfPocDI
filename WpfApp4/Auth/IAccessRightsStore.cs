namespace WpfApp4.Auth
{
    // Retrieves the access rights (permission keys) granted to a signed-in user.
    // POC implementation: ConfigurationAccessRightsStore (appsettings.json).
    // Replace with a Web API / SQL implementation and update the DI registration in App.xaml.cs.
    public interface IAccessRightsStore
    {
        Task<IReadOnlySet<string>> GetAccessRightsAsync(AppUser user, CancellationToken cancellationToken = default);
    }
}
