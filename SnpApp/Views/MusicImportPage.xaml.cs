using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SnpApp.ViewModels;
using Windows.Media.Playback;
using SnpApp.Models;
using SnpApp.Navigation;
using SnpApp.Services;
using DispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue;

namespace SnpApp.Views;

/// <summary>
///     An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MusicImportPage
{
    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    MediaPlayer Player => PlaybackService.Instance.Player;

    MediaPlaybackList? PlaybackList
    {
        get => Player.Source as MediaPlaybackList;
        set => Player.Source = value;
    }
    MediaList? MediaList
    {
        get => PlaybackService.Instance.CurrentPlaylist;
        set
        {
            if (value != null) PlaybackService.Instance.CurrentPlaylist = value;
        }
    }

    public PlayerViewModel PlayerViewModel { get; set; }
    

    /// <summary>
    ///     Initializes the page.
    /// </summary>
    public MusicImportPage()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetService<MusicImportListViewModel>();
        // Setup MediaPlayer view model
        PlayerViewModel = new PlayerViewModel(Player);
        Loaded += MusicImportPage_Loaded;
    }
    

    public MusicImportListViewModel ViewModel => (MusicImportListViewModel)DataContext;

    private async void MusicImportPage_Loaded(object sender, RoutedEventArgs e)
    {
        await ViewModel.GetListAsync(1);

        // DataGrid.ItemsSource = ViewModel.Entries;
        // DataGrid.IsReadOnly = true;
        //DataGrid.Columns[1].SortDirection = DataGridSortDirection.Ascending;
        
        
        Debug.WriteLine("MusicImportPage_Loaded");
            
        // Update controls according to settings
        UpdateControlVisibility();
        // SettingsService.Instance.UseCustomControlsChanged += SettingsService_UseCustomControlsChanged;

        // Bind player to element
        mediaPlayerElement.SetMediaPlayer(Player);

        MediaList?.Clear();

        // Load the playlist data model if needed
        if (MediaList == null)
        {
            // Create the playlist data model
            MediaList = new MediaList();
            // await MediaList.LoadFromApplicationUriAsync("ms-appx:///Assets/playlist.json");
            await MediaList.LoadFromImportMusicAsync(ViewModel.Entries.ToList());
        }
        
        await MediaList.LoadFromImportMusicAsync(ViewModel.Entries.ToList());

        // Create a new playback list matching the data model if one does not exist
        if (PlaybackList == null)
            PlaybackList = MediaList.ToPlaybackList();

        // Subscribe to playback list item failure events
        PlaybackList.ItemFailed += PlaybackList_ItemFailed;

        // Create the view model list from the data model and playback model
        // and assign it to the player
        PlayerViewModel.MediaList = new MediaListViewModel(MediaList, PlaybackList);
        
        
    }
    
    void UpdateControlVisibility()
    {
        if (SettingsService.Instance.UseCustomControls)
        {
            mediaPlayerElement.AreTransportControlsEnabled = false;
            customButtons.Visibility = Visibility.Visible;
        }
        else
        {
            mediaPlayerElement.AreTransportControlsEnabled = true;
            mediaPlayerElement.TransportControls.IsZoomButtonVisible = false;
            mediaPlayerElement.TransportControls.IsZoomEnabled = false;
            customButtons.Visibility = Visibility.Collapsed;
        }
    }
    
    
    /// <summary>
    /// Handle item failures
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private async void PlaybackList_ItemFailed(MediaPlaybackList sender, MediaPlaybackItemFailedEventArgs args)
    {
        // Media callbacks use a worker thread so dispatch to UI as needed
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            var error = $"Item failed to play: {args.Error.ErrorCode} | 0x{args.Error.ExtendedError.HResult:x}";
            NavigationRootPage.Current?.NotifyUser(error, NotifyType.ErrorMessage);
        });
    }

    /// <summary>
    ///     Initializes the AutoSuggestBox portion of the search box.
    /// </summary>
    private void SearchBox_Loaded(object sender, RoutedEventArgs e)
    {
        if (SearchBox == null) return;
        SearchBox.AutoSuggestBox.QuerySubmitted += SearchBox_QuerySubmitted;
        SearchBox.AutoSuggestBox.TextChanged += SearchBox_TextChanged;
        SearchBox.AutoSuggestBox.PlaceholderText = "Search customers...";
    }

    /// <summary>
    ///     Updates the search box items source when the user changes the search text.
    /// </summary>
    private async void SearchBox_TextChanged(AutoSuggestBox sender,
        AutoSuggestBoxTextChangedEventArgs args)
    {
        // We only want to get results when it was a user typing,
        // otherwise we assume the value got filled in by TextMemberPath
        // or the handler for SuggestionChosen.
        if (args.Reason != AutoSuggestionBoxTextChangeReason.UserInput) return;
        ViewModel.SearchRequestModel.SearchText = sender.Text;
        await _dispatcherQueue.EnqueueAsync(async () =>
            await ViewModel.GetListAsync(0));
        sender.ItemsSource = null;
    }

    /// <summary>
    ///     Filters or resets the customer list based on the search text.
    /// </summary>
    private async void SearchBox_QuerySubmitted(AutoSuggestBox sender,
        AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        ViewModel.SearchRequestModel.SearchText = args.QueryText;

        await _dispatcherQueue.EnqueueAsync(async () =>
            await ViewModel.GetListAsync(0));
    }

    /// <summary>
    ///     Resets the customer list.
    /// </summary>
    private async Task ResetList()
    {
        await _dispatcherQueue.EnqueueAsync(async () =>
            await ViewModel.GetListAsync(0));
    }

    /// <summary>
    ///     Resets the customer list when leaving the page.
    /// </summary>
    protected override async void OnNavigatedFrom(NavigationEventArgs e)
    {
        await ResetList();
    }

    /// <summary>
    ///     Sorts the data in the DataGrid.
    /// </summary>
    private async void DataGrid_Sorting(object sender, DataGridColumnEventArgs e)
    {
#pragma warning disable CS8601 // Possible null reference assignment.
        ViewModel.SearchRequestModel.SortColumn = e.Column.Tag.ToString();
#pragma warning restore CS8601 // Possible null reference assignment.
        var isAscending = e.Column.SortDirection is null or DataGridSortDirection.Descending;

        var dataGridColumns = (sender as DataGrid)?.Columns;
        if (dataGridColumns != null)
            foreach (var column in dataGridColumns)
                column.SortDirection = null;

        var direction = isAscending
            ? DataGridSortDirection.Ascending
            : DataGridSortDirection.Descending;

        ViewModel.SearchRequestModel.SortDirection = direction;

        await _dispatcherQueue.EnqueueAsync(() =>
            ViewModel.GetListAsync(0));

        //DataGrid.ItemsSource = ViewModel.Entries;
        e.Column.SortDirection = direction;
    }
}