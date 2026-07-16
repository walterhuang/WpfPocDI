using WpfApp4.ViewModels;

namespace WpfApp4.Services
{
    public interface INavigationService
    {
        void NavigateTo<TViewModel>() where TViewModel : BaseViewModel;
    }
}
