using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Snp.App.Services;

namespace Snp.App
{
    /// <summary>
    /// The "chrome" layer of the app that provides top-level navigation with
    /// proper keyboarding navigation.
    /// </summary>
    public sealed partial class Shell : Page, INavigation
    {
        /// <summary>
        /// Initializes a new instance of the AppShell, sets the static 'Current' reference,
        /// adds callbacks for Back requests and changes in the SplitView's DisplayMode, and
        /// provide the nav menu list with the data to display.
        /// </summary>
        public Shell()
        {
            // Title = App.Title;
            //
            InitializeComponent();
            //
            // var appWindow = this.GetAppWindow();
            // appWindow.SetIcon("Assets/Beer.ico");

            Root.RequestedTheme = Application.Current.RequestedTheme == ApplicationTheme.Light ? ElementTheme.Light : ElementTheme.Dark;
        }
        
        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            Root.RequestedTheme = Root.RequestedTheme == ElementTheme.Light ? ElementTheme.Dark : ElementTheme.Light;

            Ioc.Default.GetService<IMessenger>().Send(new ThemeChangedMessage(Root.ActualTheme));
        }
    }
}
