using System;
using SnpApp.Helper;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.System;
using SnpApp.Navigation;
using SnpApp.ViewModels;

namespace SnpApp
{
    /// <summary>
    /// A page that displays the app's settings.
    /// </summary>
    public sealed partial class SettingsPage
    {
        
        public SettingsViewModel ViewModel { get; set; }
        
        public string Version
        {
            get
            {
                var version = System.Reflection.Assembly.GetEntryAssembly()!.GetName().Version;
                return $"{version!.Major}.{version.Minor}.{version.Build}.{version.Revision}";
            }
        }
        
        private int _lastNavigationSelectionMode;

        public SettingsPage()
        {
            
            ViewModel = new SettingsViewModel();
            
            this.InitializeComponent();
            Loaded += OnSettingsPageLoaded;
        }

        private void OnSettingsPageLoaded(object sender, RoutedEventArgs e)
        {
            var currentTheme = ThemeHelper.RootTheme;
            switch (currentTheme)
            {
                case ElementTheme.Light:
                    themeMode.SelectedIndex = 0;
                    break;
                case ElementTheme.Dark:
                    themeMode.SelectedIndex = 1;
                    break;
                case ElementTheme.Default:
                    themeMode.SelectedIndex = 2;
                    break;
            }

            var navigationRootPage = NavigationRootPage.GetForElement(this);
            if (navigationRootPage != null)
            {
                if (navigationRootPage.NavigationView.PaneDisplayMode == NavigationViewPaneDisplayMode.Auto)
                {
                    navigationLocation.SelectedIndex = 0;
                }
                else
                {
                    navigationLocation.SelectedIndex = 1;
                }
                _lastNavigationSelectionMode = navigationLocation.SelectedIndex;
            }

            if (ElementSoundPlayer.State == ElementSoundPlayerState.On)
                soundToggle.IsOn = true;
            if (ElementSoundPlayer.SpatialAudioMode == ElementSpatialAudioMode.On)
                spatialSoundBox.IsOn = true;
        }

        private void themeMode_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var selectedTheme = ((ComboBoxItem)themeMode.SelectedItem)?.Tag?.ToString();
            var window = WindowHelper.GetWindowForElement(this);
            string color;
            if (selectedTheme != null)
            {
                ThemeHelper.RootTheme = App.GetEnum<ElementTheme>(selectedTheme);
                if (selectedTheme == "Dark")
                {
                    if (window != null) TitleBarHelper.SetCaptionButtonColors(window, Colors.White);
                    color = selectedTheme;
                }
                else if (selectedTheme == "Light")
                {
                    if (window != null) TitleBarHelper.SetCaptionButtonColors(window, Colors.Black);
                    color = selectedTheme;
                }
                else
                {
                    color = window != null && TitleBarHelper.ApplySystemThemeToCaptionButtons(window) == Colors.White  ? "Dark" : "Light";
                }
                // announce visual change to automation
                UiHelper.AnnounceActionForAccessibility((sender as UIElement)!, $"Theme changed to {color}",
                                                                                "ThemeChangedNotificationActivityId");
            }
        }

        private void soundToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (soundToggle.IsOn == true)
            {
                SpatialAudioCard.IsEnabled = true;
                ElementSoundPlayer.State = ElementSoundPlayerState.On;
            }
            else
            {
                SpatialAudioCard.IsEnabled = false;
                spatialSoundBox.IsOn = false;

                ElementSoundPlayer.State = ElementSoundPlayerState.Off;
                ElementSoundPlayer.SpatialAudioMode = ElementSpatialAudioMode.Off;
            }
        }


        private void navigationLocation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Since setting the left mode does not look at the old setting we 
            // need to check if this is an actual update
            if (navigationLocation.SelectedIndex != _lastNavigationSelectionMode)
            {
                NavigationOrientationHelper.IsLeftModeForElement(navigationLocation.SelectedIndex == 0, this);
                _lastNavigationSelectionMode = navigationLocation.SelectedIndex;
            }
        }

        private void spatialSoundBox_Toggled(object sender, RoutedEventArgs e)
        {
            if (soundToggle.IsOn == true)
            {
                ElementSoundPlayer.SpatialAudioMode = ElementSpatialAudioMode.Off;
            }
            else
            {
                ElementSoundPlayer.SpatialAudioMode = ElementSpatialAudioMode.On;
            }
        }

        private void soundPageHyperlink_Click(object sender, RoutedEventArgs e)
        {
           //  this.Frame.Navigate(typeof(ItemPage), new NavigationRootPageArgs() { Parameter = "Sound", NavigationRootPage = NavigationRootPage.GetForElement(this) });
        }

        private async void bugRequestCard_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/microsoft/WinUI-Gallery/issues/new/choose"));
        
        }
    }
}
