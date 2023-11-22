using System;
using CommunityToolkit.Mvvm.ComponentModel;
using SnpApp.Interfaces;

namespace SnpApp.Models
{
    
    public class Music : ObservableObject, IAudio,IAudioTag
    {
        private bool _isChecked;
        
        public uint Id { get; set; } = default!;
        public string FileName { get; set; } = default!;
        public ulong FileSize { get; set; } = default!;
        public string Hash { get; set; } = default!;
        public ulong Duration { get; set; } = default!;
        public string Artist { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string Album { get; set; } = default!;
        public string Genre { get; set; } = default!;
        
        public bool Enabled  { get; set; } = default!;
        
        public ulong MusicTypeId { get; set; } = default!;
        
        public DateTime Imported { get; set; } = default!;
        
        public MusicType MusicType  { get; set; } = default!;
        
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
