using Windows.Media.Core;
using SnpApp.Interfaces;

namespace SnpApp.Models
{
    
    public class MusicImport : IdObject, IAudio,IAudioTag
    {
        public string FileName { get; set; }
        public ulong FileSize { get; set; }
        public string Hash { get; set; }
        public ulong Duration { get; set; }
        public string Artist { get; set; }
        public string Title { get; set; }
        public string Album { get; set; }
        public string Genre { get; set; }
        public bool Selected { get; set; }
        
        public MediaSource Source  { get; set; }
        
        // public MediaSource Source { get; set; }
    }
}
