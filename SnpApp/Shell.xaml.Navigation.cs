using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using Snp.App.Views;

namespace Snp.App
{
    public sealed partial class Shell
    {
        private void NavigationView_Loaded(object sender, RoutedEventArgs e)
        {
            SetCurrentNavigationViewItem(GetNavigationViewItems(typeof(CustomerListPage)).First());
        }

        private void OnNavigationViewSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            
            if (args.IsSettingsSelected)
            {
                if (AppFrame.CurrentSourcePageType != typeof(SettingsPage))
                {
                    AppFrame.Navigate(typeof(SettingsPage));
                }
            }
            else
            {
                SetCurrentNavigationViewItem(args.SelectedItemContainer as NavigationViewItem);
            }
            
            
        }

        public List<NavigationViewItem> GetNavigationViewItems()
        {
            List<NavigationViewItem> result = new();
            var items =NavigationViewControl.MenuItems.Select(i => (NavigationViewItem)i).ToList();
            items.AddRange(NavigationViewControl.FooterMenuItems.Select(i => (NavigationViewItem)i));
            result.AddRange(items);

            foreach (NavigationViewItem mainItem in items)
            {
                result.AddRange(mainItem.MenuItems.Select(i => (NavigationViewItem)i));
            }

            return result;
        }

        public List<NavigationViewItem> GetNavigationViewItems(Type type)
        {
            return GetNavigationViewItems().Where(i => i.Tag.ToString() == type.FullName).ToList();
        }

        public List<NavigationViewItem> GetNavigationViewItems(Type type, string title)
        {
            return GetNavigationViewItems(type).Where(ni => ni.Content.ToString() == title).ToList();
        }

        public void SetCurrentNavigationViewItem(NavigationViewItem item)
        {
            if (item == null)
            {
                return;
            }

            if (item.Tag == null)
            {
                return;
            }

            AppFrame.Navigate(Type.GetType(item.Tag.ToString()), item.Content);
            // NavigationView.Header = item.Content;
            NavigationViewControl.SelectedItem = item;
        }

        public NavigationViewItem GetCurrentNavigationViewItem()
        {
            return NavigationViewControl.SelectedItem as NavigationViewItem;
        }
        
        /// <summary>
        /// Navigates the frame to the previous page.
        /// </summary>
        private void NavigationView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            if (AppFrame.CanGoBack)
            {
                AppFrame.GoBack();
            }
        }
    }
}
