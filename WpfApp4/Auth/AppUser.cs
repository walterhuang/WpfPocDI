namespace WpfApp4.Auth
{
    // The signed-in identity (never carries the password)
    public sealed record AppUser(string UserName, string DisplayName, IReadOnlyList<string> Roles);
}
