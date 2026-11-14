namespace WpfApp4.Auth
{
    // Holds the signed-in session and decides whether a page ViewModel may be shown
    public interface IAuthService
    {
        AppUser? CurrentUser { get; }
        bool IsAuthenticated { get; }

        // Raised after sign-in (including switching users) and sign-out
        event EventHandler? AuthStateChanged;

        Task<bool> LoginAsync(string userName, string password, CancellationToken cancellationToken = default);
        void Logout();

        bool HasPermission(string permission);
        PageAccess CheckAccess(Type pageType);
    }
}
