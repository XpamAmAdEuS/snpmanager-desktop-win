using SnpApp.ViewModels;
using System.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using SnpApp.Common;

namespace SnpApp.Controls
{
    /// <summary>
    /// A custom control for playlists that wraps a ListView.
    /// </summary>
    public sealed partial class PlaylistView: INotifyPropertyChanged
    {
        private MediaListViewModel? _mediaList;
        
        private ListViewColumnSorter? lvwColumnSorter;

        public event PropertyChangedEventHandler? PropertyChanged;

        public PlaylistView()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Raised when an item is clicked
        /// </summary>
        public event ItemClickEventHandler ItemClick
        {
            add { listView.ItemClick += value; }
            remove { listView.ItemClick -= value; }
        }

        /// <summary>
        /// Raised when the selection changes
        /// </summary>
        public event SelectionChangedEventHandler SelectionChanged
        {
            add { listView.SelectionChanged += value; }
            remove { listView.SelectionChanged -= value; }
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
