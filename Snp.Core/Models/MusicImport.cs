using Windows.Media.Playback;
using Snp.Core.Interfaces;

namespace Snp.Core.Models
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
        
        IMediaPlaybackSource Source { get; set; }
    }
}
