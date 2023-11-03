namespace Snp.App.UserControls
{
    public sealed partial class AuthenticationControl
    {
        public ViewModels.AuthenticationViewModel ViewModel { get; set; } = new ();

        public AuthenticationControl()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }
    }
}
