using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Media.Playback;

namespace SnpApp.Models
{
    public class MediaList : List<MediaItem>
    {

        public MediaList()
        { }
        
        public async Task LoadFromImportMusicAsync(List<MusicImport> items)
        {
            Clear();
            
            foreach (var item in items)
            {
                Add(LoadItemMusicImport(item));
            }
        }
        
        MediaItem LoadItemMusicImport(MusicImport item)
        {
            return new MusicItem(item);
        }

        public MediaPlaybackList ToPlaybackList()
        {
            var playbackList = new MediaPlaybackList();

            // Make a new list and enable looping
            playbackList.AutoRepeatEnabled = true;

            // Add playback items to the list
            foreach (var mediaItem in this)
            {
                playbackList.Items.Add(mediaItem.ToPlaybackItem());
            }

            return playbackList;
        }
    }
}
