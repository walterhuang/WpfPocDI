namespace WpfApp4.Auth
{
    // POC only: bound from the "Auth" section of appsettings.json.
    // The shape mirrors the future tables: Users, UserRoles, Roles, RolePermissions.
    public class AuthOptions
    {
        public const string SectionName = "Auth";

        public List<UserEntry> Users { get; set; } = [];
        public List<RoleEntry> Roles { get; set; } = [];
    }

    public class UserEntry
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = [];
    }

    public class RoleEntry
    {
        public string Name { get; set; } = string.Empty;
        public List<string> Permissions { get; set; } = [];
    }
}
