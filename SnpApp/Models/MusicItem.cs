using System;
using Windows.Media;
using Windows.Media.Playback;

namespace SnpApp.Models
{
    class MusicItem : MediaItem
    {
        private Uri? AlbumArtUri { get; set; }

        public override Uri? PreviewImageUri
        {
            get => AlbumArtUri;

            set => AlbumArtUri = value;
        }
        
        public MusicItem(MusicImport mIwm) : base(mIwm)
        {
            // if (json.Keys.Contains("albumArtUri"))
            //     AlbumArtUri = new Uri(json.GetNamedString("albumArtUri"));
        }

        public override MediaPlaybackItem ToPlaybackItem()
        {
            var playbackItem = base.ToPlaybackItem();

            var displayProperties = playbackItem.GetDisplayProperties();
            displayProperties.Type = MediaPlaybackType.Music;
            displayProperties.MusicProperties.Title = Title;
            displayProperties.MusicProperties.Artist = Artist;
            displayProperties.MusicProperties.AlbumTitle = Album;
            playbackItem.ApplyDisplayProperties(displayProperties);

            return playbackItem;
        }
    }
}
