//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using SnpApp.DataModels;
using SnpApp.Services;
using SnpApp.ViewModels;
using System;
using System.Diagnostics;
using Windows.Foundation;
using Windows.Media.Playback;
using Windows.UI.Popups;
using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SnpApp.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SnpApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario1 : Page
    {
        MediaPlayer Player => PlaybackService.Instance.Player;
        private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        MediaPlaybackList PlaybackList
        {
            get { return Player.Source as MediaPlaybackList; }
            set { Player.Source = value; }
        }
        MediaList MediaList
        {
            get { return PlaybackService.Instance.CurrentPlaylist; }
            set { PlaybackService.Instance.CurrentPlaylist = value; }
        }

        public PlayerViewModel PlayerViewModel { get; set; }

        public Scenario1()
        {
            this.InitializeComponent();

            // Never reuse the cached page because the model is designed to be unloaded and disposed
            // this.NavigationCacheMode = NavigationCacheMode.Disabled;

            // Setup MediaPlayer view model
            PlayerViewModel = new PlayerViewModel(Player);

            // Handle page load events
            Loaded += Scenario1_Loaded;
            Unloaded += Scenario1_Unloaded;

            // Respond to playback rate changes.
            Player.MediaPlayerRateChanged += Player_MediaPlayerRateChanged;
            UpdatePlaybackSpeed();
        }

        private async void Scenario1_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Scenario1_Loaded");
            
            // Update controls according to settings
            UpdateControlVisibility();
            // SettingsService.Instance.UseCustomControlsChanged += SettingsService_UseCustomControlsChanged;

            // Bind player to element
            mediaPlayerElement.SetMediaPlayer(Player);

            // Load the playlist data model if needed
            if (MediaList == null)
            {
                // Create the playlist data model
                MediaList = new MediaList();
                await MediaList.LoadFromApplicationUriAsync("ms-appx:///Assets/playlist.json");
            }

            // Create a new playback list matching the data model if one does not exist
            if (PlaybackList == null)
                PlaybackList = MediaList.ToPlaybackList();

            // Subscribe to playback list item failure events
            PlaybackList.ItemFailed += PlaybackList_ItemFailed;

            // Create the view model list from the data model and playback model
            // and assign it to the player
            PlayerViewModel.MediaList = new MediaListViewModel(MediaList, PlaybackList);
        }

        private void Scenario1_Unloaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Scenario1_Unloaded");

            // Ensure the page is no longer holding references and force a GC
            // to ensure these are unloaded immediately since the app has
            // only a short timeframe to reduce working set to avoid suspending
            // on background transition.

            // SettingsService.Instance.UseCustomControlsChanged -= SettingsService_UseCustomControlsChanged;

            PlaybackList.ItemFailed -= PlaybackList_ItemFailed;
            PlayerViewModel.Dispose();
            PlayerViewModel = null;

            GC.Collect();
        }

        private void SettingsService_UseCustomControlsChanged(object sender, EventArgs e)
        {
            UpdateControlVisibility();
        }

        void UpdateControlVisibility()
        {
            // if (SettingsService.Instance.UseCustomControls)
            // {
            //     mediaPlayerElement.AreTransportControlsEnabled = false;
            //     customButtons.Visibility = Visibility.Visible;
            // }
            // else
            // {
            //     mediaPlayerElement.AreTransportControlsEnabled = true;
            //     customButtons.Visibility = Visibility.Collapsed;
            // }
            
            mediaPlayerElement.AreTransportControlsEnabled = true;
            customButtons.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Handle item failures
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void PlaybackList_ItemFailed(MediaPlaybackList sender, MediaPlaybackItemFailedEventArgs args)
        {
            // Media callbacks use a worker thread so dispatch to UI as needed
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                var error = string.Format("Item failed to play: {0} | 0x{1:x}",
                    args.Error.ErrorCode, args.Error.ExtendedError.HResult);
                NavigationRootPage.Current.NotifyUser(error, NotifyType.ErrorMessage);
            });
        }
        
        public async void speedButton_Click(object sender, RoutedEventArgs e)
        {
            // Create menu and add commands
            var popupMenu = new PopupMenu();

            popupMenu.Commands.Add(new UICommand("4.0x", command => Player.PlaybackSession.PlaybackRate = 4.0));
            popupMenu.Commands.Add(new UICommand("2.0x", command => Player.PlaybackSession.PlaybackRate = 2.0));
            popupMenu.Commands.Add(new UICommand("1.5x", command => Player.PlaybackSession.PlaybackRate = 1.5));
            popupMenu.Commands.Add(new UICommand("1.0x", command => Player.PlaybackSession.PlaybackRate = 1.0));
            popupMenu.Commands.Add(new UICommand("0.5x", command => Player.PlaybackSession.PlaybackRate = 0.5));

            // Get button transform and then offset it by half the button
            // width to center. This will show the popup just above the button.
            var control = sender as Control;
            var transform = control.TransformToVisual(null);
            var point = transform.TransformPoint(new Point(control.ActualWidth / 2, 0));

            // Show popup
            var selected = await popupMenu.ShowAsync(point);
        }

        private async void Player_MediaPlayerRateChanged(MediaPlayer sender, MediaPlayerRateChangedEventArgs e)
        {
            // Media callbacks use a worker thread so dispatch to UI as needed
            await _dispatcherQueue.EnqueueAsync(UpdatePlaybackSpeed);
        }

        private void UpdatePlaybackSpeed()
        {
            speedButton.Content = String.Format("{0:0.0}x", Player.PlaybackSession.PlaybackRate);
        }
    }
}