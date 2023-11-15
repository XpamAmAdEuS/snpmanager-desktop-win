using System.Linq;
using Microsoft.UI.Xaml.Navigation;
using SnpApp.DataModel;
using SnpApp.Navigation;

namespace SnpApp
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class AllControlsPage
    {
        public AllControlsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var args = (NavigationRootPageArgs)e.Parameter;

            var menuItem = (Microsoft.UI.Xaml.Controls.NavigationViewItem)args.NavigationRootPage.NavigationView.MenuItems.ElementAt(2);
            menuItem.IsSelected = true;

            Items = ControlInfoDataSource.Instance.Groups.Where(g => !g.IsSpecialSection).SelectMany(g => g.Items).OrderBy(i => i.Title).ToList();
        }
    }
}
