using WpfApp4.ViewModels;

namespace WpfApp4.Services
{
    public interface INavigationService
    {
        void NavigateTo<TViewModel>(string header, bool isClosable = true) where TViewModel : BaseViewModel;
    }
}
