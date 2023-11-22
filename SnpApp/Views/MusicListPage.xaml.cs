using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SnpApp.ViewModels;

namespace SnpApp.Views;

public sealed partial class  MusicListPage
{
    
    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    
    public MusicListPage()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetService<MusicListViewModel>();
        Loaded += PaginationPage_Loaded;
    }
    
    
     public MusicListViewModel ViewModel => (MusicListViewModel)DataContext;

    private async void PaginationPage_Loaded(object sender, RoutedEventArgs e)
    {
        await ViewModel.GetMusicListAsync(1);

        DataGrid.ItemsSource = ViewModel.Musics;
        DataGrid.Columns[0].SortDirection = DataGridSortDirection.Ascending;
    }

    /// <summary>
    ///     Initializes the AutoSuggestBox portion of the search box.
    /// </summary>
    private void MusicSearchBox_Loaded(object sender, RoutedEventArgs e)
    {
        if (MusicSearchBox == null) return;
        MusicSearchBox.AutoSuggestBox.QuerySubmitted += MusicSearchBox_QuerySubmitted;
        MusicSearchBox.AutoSuggestBox.TextChanged += MusicSearchBox_TextChanged;
        MusicSearchBox.AutoSuggestBox.PlaceholderText = "Search ...";
    }

    /// <summary>
    ///     Updates the search box items source when the user changes the search text.
    /// </summary>
    private async void MusicSearchBox_TextChanged(AutoSuggestBox sender,
        AutoSuggestBoxTextChangedEventArgs args)
    {
        // We only want to get results when it was a user typing,
        // otherwise we assume the value got filled in by TextMemberPath
        // or the handler for SuggestionChosen.
        if (args.Reason != AutoSuggestionBoxTextChangeReason.UserInput) return;
        ViewModel.SearchRequestModel.SearchText = sender.Text;
        await _dispatcherQueue.EnqueueAsync(async () =>
            await ViewModel.GetMusicListAsync(0));
        sender.ItemsSource = null;
    }

    /// <summary>
    ///     Filters or resets the customer list based on the search text.
    /// </summary>
    private async void MusicSearchBox_QuerySubmitted(AutoSuggestBox sender,
        AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        ViewModel.SearchRequestModel.SearchText = args.QueryText;

        await _dispatcherQueue.EnqueueAsync(async () =>
            await ViewModel.GetMusicListAsync(0));
    }
    
    private async void DataGrid_Sorting(object sender, DataGridColumnEventArgs e)
    {

        ViewModel.SearchRequestModel.SortColumn = e.Column.Tag.ToString();

        var isAscending = e.Column.SortDirection is null or DataGridSortDirection.Descending;
        
        foreach (var column in (sender as DataGrid)?.Columns) column.SortDirection = null;


        var direction = isAscending
            ? DataGridSortDirection.Ascending
            : DataGridSortDirection.Descending;

        ViewModel.SearchRequestModel.SortDirection = direction;

        await _dispatcherQueue.EnqueueAsync(() =>
            ViewModel.GetMusicListAsync(0));

        DataGrid.ItemsSource = ViewModel.Musics;
        e.Column.SortDirection = direction;
    }
    
}