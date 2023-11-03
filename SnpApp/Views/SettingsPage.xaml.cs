using System;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.System;
using Snp.App.Helper;

namespace Snp.App.Views
{
    public sealed partial class SettingsPage
    {
        
        public string Version => "1.0.0";

        /// <summary>
        /// Initializes a new instance of the SettingsPage class.
        /// </summary>
        public SettingsPage()
        {
            InitializeComponent();
            
            Loaded += OnSettingsPageLoaded;

            if (ElementSoundPlayer.State == ElementSoundPlayerState.On)
                soundToggle.IsOn = true;
            if (ElementSoundPlayer.SpatialAudioMode == ElementSpatialAudioMode.On)
                spatialSoundBox.IsOn = true;
        }
        
        
        // protected override void OnNavigatedTo(NavigationEventArgs e)
        // {
        //     base.OnNavigatedTo(e);
        // }

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

            var navigationRootPage = AppShell.GetForElement(this);
            if (navigationRootPage == null) return;
            navigationLocation.SelectedIndex = navigationRootPage.NavigationView.PaneDisplayMode == NavigationViewPaneDisplayMode.Auto ? 0 : 1;
        }

        private void themeMode_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var selectedTheme = ((ComboBoxItem)themeMode.SelectedItem)?.Tag?.ToString();
            var window = WindowHelper.GetWindowForElement(this);
            if (selectedTheme == null) return;
            ThemeHelper.RootTheme = App.GetEnum<ElementTheme>(selectedTheme);
            switch (selectedTheme)
            {
                case "Dark":
                    TitleBarHelper.SetCaptionButtonColors(window, Colors.White);
                    break;
                case "Light":
                    TitleBarHelper.SetCaptionButtonColors(window, Colors.Black);
                    break;
            }
        }

        private void soundToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (soundToggle.IsOn)
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
            NavigationOrientationHelper.IsLeftModeForElement(navigationLocation.SelectedIndex == 0, this);
        }

        private void spatialSoundBox_Toggled(object sender, RoutedEventArgs e)
        {
            ElementSoundPlayer.SpatialAudioMode = soundToggle.IsOn ? ElementSpatialAudioMode.Off : ElementSpatialAudioMode.On;
        }

        private void soundPageHyperlink_Click(object sender, RoutedEventArgs e)
        {
            // Frame.Navigate(typeof(ItemPage), new NavigationRootPageArgs() { Parameter = "Sound", NavigationRootPage = NavigationRootPage.GetForElement(this) });
        }

        private async void bugRequestCard_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/microsoft/WinUI-Gallery/issues/new/choose"));

        }
    }
}
