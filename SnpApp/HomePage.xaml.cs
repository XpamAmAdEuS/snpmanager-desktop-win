using System.Linq;
using Microsoft.UI.Xaml.Navigation;
using SnpApp.Navigation;

namespace SnpApp
{
    public sealed partial class HomePage
    {
        public HomePage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var args = (NavigationRootPageArgs)e.Parameter;
            if (args.NavigationRootPage == null) return;
            var menuItem = (Microsoft.UI.Xaml.Controls.NavigationViewItem)args.NavigationRootPage.NavigationView.MenuItems.First();
            if (menuItem != null) menuItem.IsSelected = true;
        }
    }
}
