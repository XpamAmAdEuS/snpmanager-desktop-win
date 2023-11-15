using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.System;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using SnpApp.ViewModels;
using Windows.Media.Playback;
using SnpApp.DataModels;
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

    MediaPlaybackList PlaybackList
    {
        get { return Player.Source as MediaPlaybackList; }
        set { Player.Source = value; }
    }
    MediaList MediaList
    {
        get { return PlaybackService.Instance.CurrentPlaylist; }
        set { PlaybackService.Instance.CurrentPlaylist = value; }
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
        Unloaded += MusicImportPage_Unloaded;
    }
    

    public MusicImportListViewModel ViewModel => (MusicImportListViewModel)DataContext;

    private async void MusicImportPage_Loaded(object sender, RoutedEventArgs e)
    {
        await ViewModel.GetListAsync(1);

        DataGrid.ItemsSource = ViewModel.Musics;
        DataGrid.Columns[0].SortDirection = DataGridSortDirection.Ascending;
        
        
        Debug.WriteLine("MusicImportPage_Loaded");
            
        // Update controls according to settings
        UpdateControlVisibility();
        // SettingsService.Instance.UseCustomControlsChanged += SettingsService_UseCustomControlsChanged;

        // Bind player to element
        mediaPlayerElement.SetMediaPlayer(Player);

        // Load the playlist data model if needed
        if (MediaList == null)
        {
            // Create the playlist data model
            MediaList = new MediaList();
            // await MediaList.LoadFromApplicationUriAsync("ms-appx:///Assets/playlist.json");
            await MediaList.LoadFromImportMusicAsync(ViewModel.Musics);
        }

        // Create a new playback list matching the data model if one does not exist
        if (PlaybackList == null)
            PlaybackList = MediaList.ToPlaybackList();

        // Subscribe to playback list item failure events
        PlaybackList.ItemFailed += PlaybackList_ItemFailed;

        // Create the view model list from the data model and playback model
        // and assign it to the player
        PlayerViewModel.MediaList = new MediaListViewModel(MediaList, PlaybackList);
        
        
    }
    
    private void MusicImportPage_Unloaded(object sender, RoutedEventArgs e)
    {
        Debug.WriteLine("MusicImportPage_Unloaded");

        // Ensure the page is no longer holding references and force a GC
        // to ensure these are unloaded immediately since the app has
        // only a short timeframe to reduce working set to avoid suspending
        // on background transition.

        // SettingsService.Instance.UseCustomControlsChanged -= SettingsService_UseCustomControlsChanged;

        PlaybackList.ItemFailed -= PlaybackList_ItemFailed;
        PlayerViewModel.Dispose();
        PlayerViewModel = null;

        GC.Collect();
    }
    
    void UpdateControlVisibility()
    {
        // if (SettingsService.Instance.UseCustomControls)
        // {
        //     mediaPlayerElement.AreTransportControlsEnabled = false;
        //     customButtons.Visibility = Visibility.Visible;
        // }
        // else
        // {
        //     mediaPlayerElement.AreTransportControlsEnabled = true;
        //     customButtons.Visibility = Visibility.Collapsed;
        // }
            
        mediaPlayerElement.AreTransportControlsEnabled = true;
        mediaPlayerElement.TransportControls.IsZoomButtonVisible = false;
        mediaPlayerElement.TransportControls.IsZoomEnabled = false;
        mediaPlayerElement.TransportControls.IsPlaybackRateButtonVisible = true;
        mediaPlayerElement.TransportControls.IsPlaybackRateEnabled = true;
        
        customButtons.Visibility = Visibility.Collapsed;
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
            var error = string.Format("Item failed to play: {0} | 0x{1:x}",
                args.Error.ErrorCode, args.Error.ExtendedError.HResult);
            NavigationRootPage.Current.NotifyUser(error, NotifyType.ErrorMessage);
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
    ///     Applies any existing filter when navigating to the page.
    /// </summary>
    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
    }

    /// <summary>
    ///     Menu flyout click control for selecting a customer and displaying details.
    /// </summary>
    private void ViewDetails_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.SelectedMusic != null)
        {
            // NavigationRootPage.GetForElement(this).Navigate(typeof(MusicImportDetailPage), ViewModel.SelectedMusic.Model.Id, new DrillInNavigationTransitionInfo());
            
        }
            
    }

    private void DataGrid_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
        // Frame.Navigate(typeof(DetailPage), ViewModel.SelectedMusic.Model.Id,
        //     new DrillInNavigationTransitionInfo());
    }

    /// <summary>
    ///     Reverts all changes to the row if the row has changes but a cell is not currently in edit mode.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DataGrid_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == VirtualKey.Escape &&
            ViewModel.SelectedMusic is { IsModified: true, IsInEdit: false })
            (sender as DataGrid)?.CancelEdit(DataGridEditingUnit.Row);
    }

    /// <summary>
    ///     Selects the tapped customer.
    /// </summary>
    private void DataGrid_RightTapped(object sender, RightTappedRoutedEventArgs e)
    {
        ViewModel.SelectedMusic = (e.OriginalSource as FrameworkElement)?.DataContext as MusicImportViewModel;
    }


    private void AddSite_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.SelectedMusic != null)
            Frame.Navigate(typeof(SiteDetailPage), ViewModel.SelectedMusic.Model.Id,
                new DrillInNavigationTransitionInfo());
    }
    
    public void Play_Row_Click(object sender, RoutedEventArgs e)
    {
        
        MusicImportViewModel model = (sender as Button).DataContext as MusicImportViewModel;

        ViewModel.Play(model.Model);

    }

    /// <summary>
    ///     Sorts the data in the DataGrid.
    /// </summary>
    private async void DataGrid_Sorting(object sender, DataGridColumnEventArgs e)
    {
        ViewModel.SearchRequestModel.SortColumn = e.Column.Tag.ToString();
        var isAscending = e.Column.SortDirection is null or DataGridSortDirection.Descending;

        foreach (var column in (sender as DataGrid).Columns) column.SortDirection = null;

        var direction = isAscending
            ? DataGridSortDirection.Ascending
            : DataGridSortDirection.Descending;

        ViewModel.SearchRequestModel.SortDirection = direction;

        await _dispatcherQueue.EnqueueAsync(() =>
            ViewModel.GetListAsync(0));

        DataGrid.ItemsSource = ViewModel.Musics;
        e.Column.SortDirection = direction;
    }
}