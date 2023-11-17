using CommunityToolkit.Mvvm.ComponentModel;
using SnpApp.Interfaces;

namespace SnpApp.Models
{
    
    public class MusicImport : ObservableObject, IAudio,IAudioTag
    {
        private bool _isChecked;
        
        public uint Id { get; set; }
        public string FileName { get; set; }
        public ulong FileSize { get; set; }
        public string Hash { get; set; }
        public ulong Duration { get; set; }
        public string Artist { get; set; }
        public string Title { get; set; }
        public string Album { get; set; }
        public string Genre { get; set; }
        
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
    }
}
