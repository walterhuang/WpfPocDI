namespace WpfApp4.ViewModels
{
    public class TabItemViewModel : BaseViewModel
    {
        private BaseViewModel? _content;

        public TabItemViewModel(string header, Type pageType, bool isClosable)
        {
            Header = header;
            PageType = pageType;
            IsClosable = isClosable;
        }

        public string Header { get; }

        // The requested page; identifies the tab even while it shows Login or Access Denied
        public Type PageType { get; }

        // The page itself, or a LoginViewModel / AccessDeniedViewModel standing in for it
        public BaseViewModel? Content
        {
            get => _content;
            set => SetProperty(ref _content, value);
        }

        public bool IsClosable { get; }
    }
}
