using System;
using System.ComponentModel;
using Windows.Media.Playback;
using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;

namespace SnpApp.ViewModels
{
    public class PlaybackSessionViewModel : INotifyPropertyChanged, IDisposable
    {
        bool disposed;
        MediaPlayer player;
        MediaPlaybackSession playbackSession;
        DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        public MediaPlaybackState PlaybackState => playbackSession.PlaybackState;
        public event PropertyChangedEventHandler? PropertyChanged;

        public PlaybackSessionViewModel(MediaPlaybackSession playbackSession)
        {
            this.player = playbackSession.MediaPlayer;
            this.playbackSession = playbackSession;

            playbackSession.PlaybackStateChanged += PlaybackSession_PlaybackStateChanged;
        }
 
        private async void PlaybackSession_PlaybackStateChanged(MediaPlaybackSession sender, object args)
        {
            if (disposed) return;
            await dispatcherQueue.EnqueueAsync(() =>
            {
                if (disposed) return;
                RaisePropertyChanged("PlaybackState");
            });
        }

        public void Dispose()
        {
            if (disposed)
                return;

            playbackSession.PlaybackStateChanged -= PlaybackSession_PlaybackStateChanged;

            disposed = true;
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
