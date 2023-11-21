using System;
using System.ComponentModel;
using Windows.Foundation.Collections;
using Windows.Media.Playback;
using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;

namespace SnpApp.ViewModels
{
    /// <summary>
    /// The view model for the player.
    /// </summary>
    /// <remarks>
    /// The view disables the ability to skip during a transition or when
    /// the playback list is empty.
    /// </remarks>
    public class PlayerViewModel : INotifyPropertyChanged, IDisposable
    {
        private bool _disposed;
        private readonly MediaPlayer _player;
        private MediaPlaybackList _subscribedPlaybackList;
        private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        private MediaListViewModel _mediaList;
        private bool _canSkipNext;
        private bool _canSkipPrevious;

        public PlaybackSessionViewModel PlaybackSession { get; private set; }

        public bool CanSkipNext
        {
            get => _canSkipNext;
            set
            {
                if (_canSkipNext != value)
                {
                    _canSkipNext = value;
                    RaisePropertyChanged(nameof(CanSkipNext));
                }
            }
        }

        public bool CanSkipPrevious
        {
            get => _canSkipPrevious;
            set
            {
                if (_canSkipPrevious != value)
                {
                    _canSkipPrevious = value;
                    RaisePropertyChanged(nameof(CanSkipPrevious));
                }
            }
        }

        public MediaListViewModel MediaList
        {
            get => _mediaList;
            set
            {
                if (_mediaList != value)
                {
                    if (_subscribedPlaybackList != null)
                    {
                        _subscribedPlaybackList.CurrentItemChanged -= SubscribedPlaybackList_CurrentItemChanged;
                        _subscribedPlaybackList.Items.VectorChanged -= Items_VectorChanged;
                        _subscribedPlaybackList = null;
                    }

                    _mediaList = value;

                    if (_mediaList != null)
                    {
                        if (_player.Source != _mediaList.PlaybackList)
                            _player.Source = _mediaList.PlaybackList;

                        _subscribedPlaybackList = _mediaList.PlaybackList;
                        _subscribedPlaybackList.CurrentItemChanged += SubscribedPlaybackList_CurrentItemChanged;
                        _subscribedPlaybackList.Items.VectorChanged += Items_VectorChanged;
                        HandlePlaybackListChanges(_subscribedPlaybackList.Items);
                    }
                    else
                    {
                        CanSkipNext = false;
                        CanSkipPrevious = false;
                    }

                    RaisePropertyChanged(nameof(MediaList));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public PlayerViewModel(MediaPlayer player)
        {
            this._player = player;
            PlaybackSession = new PlaybackSessionViewModel(player.PlaybackSession);
        }

        public void TogglePlayPause()
        {
            switch (_player.PlaybackSession.PlaybackState)
            {
                case MediaPlaybackState.Playing:
                    _player.Pause();
                    break;
                case MediaPlaybackState.Paused:
                    _player.Play();
                    break;
            }
        }

        public void SkipNext()
        {
            if (!CanSkipNext)
                return;

            var playbackList = _player.Source as MediaPlaybackList;
            if (playbackList == null)
                return;

            playbackList.MoveNext();
            CanSkipNext = false;
        }

        public void SkipPrevious()
        {
            if (!CanSkipPrevious)
                return;

            var playbackList = _player.Source as MediaPlaybackList;
            if (playbackList == null)
                return;

            playbackList.MovePrevious();
            CanSkipPrevious = false;
        }

        private async void Items_VectorChanged(IObservableVector<MediaPlaybackItem> sender, IVectorChangedEventArgs args)
        {
            if (_disposed) return;
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                if (_disposed) return;
                HandlePlaybackListChanges(sender);
            });
        }

        private async void SubscribedPlaybackList_CurrentItemChanged(MediaPlaybackList sender, CurrentMediaPlaybackItemChangedEventArgs args)
        {
            if (_disposed) return;
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                if (_disposed) return;
                HandlePlaybackListChanges(sender.Items);
            });
        }

        private void HandlePlaybackListChanges(IObservableVector<MediaPlaybackItem> vector)
        {
            if (vector.Count > 0)
            {
                CanSkipNext = true;
                CanSkipPrevious = true;
            }
            else
            {
                CanSkipNext = false;
                CanSkipPrevious = false;
            }
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
      
        public void Dispose()
        {
            if (_disposed)
                return;

            if (MediaList != null)
            {
                MediaList.Dispose();
                MediaList = null; // Setter triggers vector unsubscribe logic
            }

            PlaybackSession.Dispose();

            _disposed = true;
        }
    }
}
