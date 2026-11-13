using Microsoft.Extensions.Options;

namespace WpfApp4.Auth
{
    // POC only: access rights are the union of the permissions of the user's roles in "Auth:Roles"
    public class ConfigurationAccessRightsStore : IAccessRightsStore
    {
        private readonly IOptionsMonitor<AuthOptions> _options;

        public ConfigurationAccessRightsStore(IOptionsMonitor<AuthOptions> options)
        {
            _options = options;
        }

        public Task<IReadOnlySet<string>> GetAccessRightsAsync(AppUser user, CancellationToken cancellationToken = default)
        {
            var userRoles = new HashSet<string>(user.Roles, StringComparer.OrdinalIgnoreCase);

            var rights = _options.CurrentValue.Roles
                .Where(r => userRoles.Contains(r.Name))
                .SelectMany(r => r.Permissions)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            return Task.FromResult<IReadOnlySet<string>>(rights);
        }
    }
}
