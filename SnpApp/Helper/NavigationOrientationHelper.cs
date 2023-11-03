using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage;


namespace Snp.App.Helper
{
    public static class NavigationOrientationHelper
    {

        private const string IsLeftModeKey = "NavigationIsOnLeftMode";
        private static bool _isLeftMode = true;

        public static bool IsLeftMode()
        {
            if (NativeHelper.IsAppPackaged)
            {
                var valueFromSettings = ApplicationData.Current.LocalSettings.Values[IsLeftModeKey];
                if (valueFromSettings == null)
                {
                    ApplicationData.Current.LocalSettings.Values[IsLeftModeKey] = true;
                    valueFromSettings = true;
                }
                return (bool)valueFromSettings;
            }
            else
            {
                return _isLeftMode;
            }
        }

        public static void IsLeftModeForElement(bool isLeftMode, UIElement element)
        {
            UpdateNavigationViewForElement(isLeftMode, element);
            if (NativeHelper.IsAppPackaged)
            {
                ApplicationData.Current.LocalSettings.Values[IsLeftModeKey] = isLeftMode;
            }
            else
            {
                _isLeftMode = isLeftMode;
            }
        }

        public static void UpdateNavigationViewForElement(bool isLeftMode, UIElement element)
        {
            var navView = AppShell.GetForElement(element).NavigationView;
            if (isLeftMode)
            {
                navView.PaneDisplayMode = NavigationViewPaneDisplayMode.Auto;
                Grid.SetRow(navView, 0);
            }
            else
            {
                navView.PaneDisplayMode = NavigationViewPaneDisplayMode.Top;
                Grid.SetRow(navView, 1);
            }
        }
        
    }
}
