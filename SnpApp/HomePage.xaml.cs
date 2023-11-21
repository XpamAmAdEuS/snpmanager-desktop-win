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
            var menuItem = (Microsoft.UI.Xaml.Controls.NavigationViewItem)args.NavigationRootPage.NavigationView.MenuItems.First();
            menuItem.IsSelected = true;
        }

        protected bool GetIsNarrowLayoutState()
        {
            return LayoutVisualStates.CurrentState == NarrowLayout;
        }
    }
}
