namespace WpfApp4.Auth
{
    // Marks a page ViewModel as viewable without signing in. Pages without it require login.
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class AllowAnonymousAttribute : Attribute
    {
    }
}
