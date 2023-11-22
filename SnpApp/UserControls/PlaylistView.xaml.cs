using SnpApp.ViewModels;
using System.ComponentModel;

namespace SnpApp.UserControls
{
    
    public sealed partial class PlaylistView: INotifyPropertyChanged
    {
        private MediaListViewModel? _mediaList;

        public event PropertyChangedEventHandler? PropertyChanged;

        public PlaylistView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// A collection of songs in the list to be displayed to the user
        /// </summary>
        public MediaListViewModel? MediaList
        {
            get => _mediaList;
            set
            {
                if (_mediaList != value)
                {
                    _mediaList = value;

                    // Setting ItemsSource = null on page unload throws, so avoid that
                    if (value != null)
                        listView.ItemsSource = _mediaList;

                    RaisePropertyChanged(nameof(MediaList));
                }
            }
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
