namespace WpfApp4.Services
{
    public interface IAuthService
    {
        bool Validate(string username, string password);
    }
}
