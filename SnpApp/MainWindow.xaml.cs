using Microsoft.UI.Xaml;
using SnpApp.Services;

namespace SnpApp
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            
            Title = App.Title;

            InitializeComponent();

            var appWindow = this.GetAppWindow();
            appWindow.SetIcon("Assets/Beer.ico");
            
        }
    }
}
