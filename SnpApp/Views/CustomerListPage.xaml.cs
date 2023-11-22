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
using SnpApp.Navigation;
using SnpApp.ViewModels;
using DispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue;

namespace SnpApp.Views;

/// <summary>
///     An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class CustomerListPage
{
    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

    /// <summary>
    ///     Initializes the page.
    /// </summary>
    public CustomerListPage()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetService<CustomerListViewModel>();
        Loaded += PaginationPage_Loaded;
    }

    public CustomerListViewModel ViewModel => (CustomerListViewModel)DataContext;

    private async void PaginationPage_Loaded(object sender, RoutedEventArgs e)
    {
        await ViewModel.GetCustomerListAsync(1);

        DataGrid.ItemsSource = ViewModel.Customers;
        DataGrid.Columns[0].SortDirection = DataGridSortDirection.Ascending;
    }

    /// <summary>
    ///     Initializes the AutoSuggestBox portion of the search box.
    /// </summary>
    private void CustomerSearchBox_Loaded(object sender, RoutedEventArgs e)
    {
        if (CustomerSearchBox == null) return;
        CustomerSearchBox.AutoSuggestBox.QuerySubmitted += CustomerSearchBox_QuerySubmitted;
        CustomerSearchBox.AutoSuggestBox.TextChanged += CustomerSearchBox_TextChanged;
        CustomerSearchBox.AutoSuggestBox.PlaceholderText = "Search customers...";
    }

    /// <summary>
    ///     Updates the search box items source when the user changes the search text.
    /// </summary>
    private async void CustomerSearchBox_TextChanged(AutoSuggestBox sender,
        AutoSuggestBoxTextChangedEventArgs args)
    {
        // We only want to get results when it was a user typing,
        // otherwise we assume the value got filled in by TextMemberPath
        // or the handler for SuggestionChosen.
        if (args.Reason != AutoSuggestionBoxTextChangeReason.UserInput) return;
        ViewModel.SearchRequestModel.SearchText = sender.Text;
        await _dispatcherQueue.EnqueueAsync(async () =>
            await ViewModel.GetCustomerListAsync(0));
        sender.ItemsSource = null;
    }

    /// <summary>
    ///     Filters or resets the customer list based on the search text.
    /// </summary>
    private async void CustomerSearchBox_QuerySubmitted(AutoSuggestBox sender,
        AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        ViewModel.SearchRequestModel.SearchText = args.QueryText;

        await _dispatcherQueue.EnqueueAsync(async () =>
            await ViewModel.GetCustomerListAsync(0));
    }

    /// <summary>
    ///     Resets the customer list.
    /// </summary>
    private async Task ResetCustomerList()
    {
        await _dispatcherQueue.EnqueueAsync(async () =>
            await ViewModel.GetCustomerListAsync(0));
    }

    /// <summary>
    ///     Resets the customer list when leaving the page.
    /// </summary>
    protected override async void OnNavigatedFrom(NavigationEventArgs e)
    {
        await ResetCustomerList();
    }

    /// <summary>
    ///     Menu flyout click control for selecting a customer and displaying details.
    /// </summary>
    private void ViewDetails_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.SelectedCustomer != null)
        {
            NavigationRootPage.GetForElement(this)?.Navigate(typeof(CustomerDetailPage), ViewModel.SelectedCustomer.Model.Id, new DrillInNavigationTransitionInfo());
            
            // Frame.Navigate(typeof(CustomerDetailPage), ViewModel.SelectedCustomer.Model.Id,
            //     new DrillInNavigationTransitionInfo());
            
        }
            
    }

    private void DataGrid_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
        // Frame.Navigate(typeof(CustomerDetailPage), ViewModel.SelectedCustomer.Model.Id,
        //     new DrillInNavigationTransitionInfo());
    }

    /// <summary>
    ///     Navigates to a blank customer details page for the user to fill in.
    /// </summary>
    private void CreateCustomer_Click(object sender, RoutedEventArgs e)
    {
        Frame.Navigate(typeof(CustomerDetailPage), null, new DrillInNavigationTransitionInfo());
    }

    /// <summary>
    ///     Reverts all changes to the row if the row has changes but a cell is not currently in edit mode.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DataGrid_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == VirtualKey.Escape &&
            ViewModel.SelectedCustomer is { IsModified: true, IsInEdit: false })
            (sender as DataGrid)?.CancelEdit(DataGridEditingUnit.Row);
    }

    /// <summary>
    ///     Selects the tapped customer.
    /// </summary>
    private void DataGrid_RightTapped(object sender, RightTappedRoutedEventArgs e)
    {
        ViewModel.SelectedCustomer = (e.OriginalSource as FrameworkElement)?.DataContext as CustomerViewModel;
    }


    private void AddSite_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.SelectedCustomer != null)
            Frame.Navigate(typeof(SiteDetailPage), ViewModel.SelectedCustomer.Model.Id,
                new DrillInNavigationTransitionInfo());
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

#pragma warning disable CS8602 // Dereference of a possibly null reference.
        foreach (var column in (sender as DataGrid).Columns) column.SortDirection = null;
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        var direction = isAscending
            ? DataGridSortDirection.Ascending
            : DataGridSortDirection.Descending;

        ViewModel.SearchRequestModel.SortDirection = direction;

        await _dispatcherQueue.EnqueueAsync(() =>
            ViewModel.GetCustomerListAsync(0));

        DataGrid.ItemsSource = ViewModel.Customers;
        e.Column.SortDirection = direction;
    }
}