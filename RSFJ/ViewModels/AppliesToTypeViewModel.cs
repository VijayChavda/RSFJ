namespace RSFJ.ViewModels
{
    public class AppliesToTypeViewModel : ViewModelBase
    {
        public bool Applies { get => _Applies; set => SetProperty(ref _Applies, value); }
        private bool _Applies;

        public string Type { get => _Type; set => SetProperty(ref _Type, value); }
        private string _Type;
    }
}
