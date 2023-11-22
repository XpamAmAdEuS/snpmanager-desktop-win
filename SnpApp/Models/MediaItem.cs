using System;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage.Streams;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SnpApp.Models
{
    

    public class MediaItem : ObservableObject
    {
        public const string MediaItemIdKey = "mediaItemId";
        
        private bool _isChecked;

        public string ItemId { get; set; } = default!;

        public string Title { get; set; } = default!;
        
        public string Artist { get; set; } = default!;
        
        public string Album { get; set; } = default!;
        
        public string Hash { get; set; } = default!;
        
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (value == _isChecked) return;
                _isChecked = value;
                OnPropertyChanged();
            }
        }

        public Uri MediaUri { get; set; } = default!;

        public virtual Uri? PreviewImageUri { get; set; } = default!;

        public MediaItem() {}

        protected MediaItem(MusicImport mIwm) : this()
        {
            ItemId = mIwm.Id.ToString();
            Title = mIwm.Title;
            Artist = mIwm.Artist;
            Album = mIwm.Album;
            IsChecked = mIwm.IsChecked;
            Hash = mIwm.Hash;
            MediaUri = new Uri($"http://192.168.1.36:50051/v1/music/import/file/{mIwm.Hash}.mp3");
        }

        public virtual MediaPlaybackItem ToPlaybackItem()
        {
            // Create the media source from the Uri
            var source = MediaSource.CreateFromUri(MediaUri);

            // Create a configurable playback item backed by the media source
            var playbackItem = new MediaPlaybackItem(source);

            // Populate display properties for the item that will be used
            // to automatically update SystemMediaTransportControls when
            // the item is playing.
            var displayProperties = playbackItem.GetDisplayProperties();

            // Populate thumbnail
            if (PreviewImageUri != null)
                displayProperties.Thumbnail = RandomAccessStreamReference.CreateFromUri(PreviewImageUri);

            // Apply properties to the playback item
            playbackItem.ApplyDisplayProperties(displayProperties);

            // It's often useful to save a reference or ID to correlate
            // a particular MediaPlaybackItem with the item from the
            // backing data model. CustomProperties stores serializable
            // types, so here we use the media item's URI as the
            // playback item's unique ID. You are also free to use your own
            // external dictionary if you want to reference non-serializable
            // types.
            source.CustomProperties[MediaItemIdKey] = ItemId;

            return playbackItem;
        }
    }
}
