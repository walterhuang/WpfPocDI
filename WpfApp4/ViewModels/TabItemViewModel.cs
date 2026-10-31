namespace WpfApp4.ViewModels
{
    public class TabItemViewModel : BaseViewModel
    {
        public TabItemViewModel(string header, BaseViewModel content, bool isClosable)
        {
            Header = header;
            Content = content;
            IsClosable = isClosable;
        }

        public string Header { get; }
        public BaseViewModel Content { get; }
        public Type ViewModelType => Content.GetType();
        public bool IsClosable { get; }
    }
}
