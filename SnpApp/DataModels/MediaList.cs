using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Data.Json;
using Windows.Media.Playback;
using SnpApp.ViewModels;

namespace SnpApp.DataModels
{
    public class MediaList : List<MediaItem>
    {
        public string Title { get; set; }

        public MediaList()
        {
        }

        public async Task LoadFromApplicationUriAsync(string uri)
        {
            await LoadFromApplicationUriAsync(new Uri(uri));
        }
        
        public async Task LoadFromImportMusicAsync(ObservableCollection<MusicImportViewModel> items)
        {
            Clear();
            Title = "Import Music";
            
            foreach (var item in items)
            {
                Add(LoadItemMusicImport(item));
            }
        }
        
        MediaItem LoadItemMusicImport(MusicImportViewModel item)
        {

            return new MusicItem(item);
        }

        public async Task LoadFromApplicationUriAsync(Uri uri)
        {
            Clear();

            var storageFile = await StorageFile.GetFileFromApplicationUriAsync(uri);
            var jsonText = await FileIO.ReadTextAsync(storageFile);
            var json = JsonObject.Parse(jsonText);
            json = json.GetNamedObject("mediaList");

            Title = json["title"].GetString();
            var items = json["items"].GetArray();
            foreach (var jsonItem in items)
            {
                Add(LoadItem(jsonItem.GetObject()));
            }
        }

        MediaItem LoadItem(JsonObject json)
        {
            var mediaType = json["mediaType"].GetString();
            switch (mediaType)
            {
                case "music":
                    return new MusicItem(json);
                default:
                    return new MediaItem(json);
            }
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
