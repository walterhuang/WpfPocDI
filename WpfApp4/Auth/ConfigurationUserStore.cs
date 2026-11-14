using Microsoft.Extensions.Options;

namespace WpfApp4.Auth
{
    // POC only: users and plain-text passwords come from the "Auth:Users" section of appsettings.json
    public class ConfigurationUserStore : IUserStore
    {
        private readonly IOptionsMonitor<AuthOptions> _options;

        public ConfigurationUserStore(IOptionsMonitor<AuthOptions> options)
        {
            _options = options;
        }

        public Task<AppUser?> ValidateCredentialsAsync(string userName, string password, CancellationToken cancellationToken = default)
        {
            var name = userName.Trim();
            if (name.Length == 0 || string.IsNullOrEmpty(password))
                return Task.FromResult<AppUser?>(null);

            var entry = _options.CurrentValue.Users.FirstOrDefault(u =>
                string.Equals(u.UserName, name, StringComparison.OrdinalIgnoreCase));

            if (entry is null || !string.Equals(entry.Password, password, StringComparison.Ordinal))
                return Task.FromResult<AppUser?>(null);

            var displayName = string.IsNullOrWhiteSpace(entry.DisplayName) ? entry.UserName : entry.DisplayName;
            return Task.FromResult<AppUser?>(new AppUser(entry.UserName, displayName, entry.Roles.ToArray()));
        }
    }
}
