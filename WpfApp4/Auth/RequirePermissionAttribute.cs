namespace WpfApp4.Auth
{
    // Marks a page ViewModel as requiring the given access right in addition to being signed in
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class RequirePermissionAttribute : Attribute
    {
        public RequirePermissionAttribute(string permission) => Permission = permission;

        public string Permission { get; }
    }
}
