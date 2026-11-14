using Microsoft.Extensions.Logging;
using System.Reflection;

namespace WpfApp4.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUserStore _userStore;
        private readonly IAccessRightsStore _accessRightsStore;
        private readonly ILogger<AuthService> _logger;
        private IReadOnlySet<string> _accessRights = new HashSet<string>();

        public AuthService(IUserStore userStore, IAccessRightsStore accessRightsStore, ILogger<AuthService> logger)
        {
            _userStore = userStore;
            _accessRightsStore = accessRightsStore;
            _logger = logger;
        }

        public AppUser? CurrentUser { get; private set; }
        public bool IsAuthenticated => CurrentUser is not null;

        public event EventHandler? AuthStateChanged;

        public async Task<bool> LoginAsync(string userName, string password, CancellationToken cancellationToken = default)
        {
            // No ConfigureAwait(false): AuthStateChanged handlers update UI-bound collections,
            // so the event must be raised back on the UI thread.
            var user = await _userStore.ValidateCredentialsAsync(userName, password, cancellationToken);
            if (user is null)
            {
                // A failed attempt leaves the current session (if any) untouched
                _logger.LogWarning("Sign-in failed for {UserName}", userName);
                return false;
            }

            var rights = await _accessRightsStore.GetAccessRightsAsync(user, cancellationToken);

            CurrentUser = user;
            _accessRights = new HashSet<string>(rights, StringComparer.OrdinalIgnoreCase);
            _logger.LogInformation("{UserName} signed in with access rights: {AccessRights}",
                user.UserName, string.Join(", ", _accessRights));

            AuthStateChanged?.Invoke(this, EventArgs.Empty);
            return true;
        }

        public void Logout()
        {
            if (CurrentUser is null) return;

            _logger.LogInformation("{UserName} signed out", CurrentUser.UserName);
            CurrentUser = null;
            _accessRights = new HashSet<string>();

            AuthStateChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool HasPermission(string permission) => IsAuthenticated && _accessRights.Contains(permission);

        // Secure by default: every page requires sign-in unless marked [AllowAnonymous]
        public PageAccess CheckAccess(Type pageType)
        {
            if (pageType.IsDefined(typeof(AllowAnonymousAttribute), inherit: true))
                return PageAccess.Allowed;

            if (!IsAuthenticated)
                return PageAccess.LoginRequired;

            var required = pageType.GetCustomAttribute<RequirePermissionAttribute>(inherit: true);
            return required is null || HasPermission(required.Permission)
                ? PageAccess.Allowed
                : PageAccess.Forbidden;
        }
    }
}
