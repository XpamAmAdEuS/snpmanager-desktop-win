using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;
using Snp.Core.Models;
using Snp.Core.Repository;

namespace Snp.Core.ViewModels;

/// <summary>
///     Provides data and commands accessible to the entire app.
/// </summary>
public class CustomerListViewModel : ObservableObject
{
    private bool _isLoading;
    private int _pageCount;

    private int _pageNumber;

    private CustomerViewModel _selectedCustomer;
    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

    public CustomerListViewModel()
    {
        FirstAsyncCommand = new AsyncRelayCommand(
            async () => await GetCustomerListAsync(1),
            () => _pageNumber != 1
        );
        PreviousAsyncCommand = new AsyncRelayCommand(
            async () => await GetCustomerListAsync(_pageNumber - 1),
            () => _pageNumber > 1
        );
        NextAsyncCommand = new AsyncRelayCommand(
            async () => await GetCustomerListAsync(_pageNumber + 1),
            () => _pageNumber < _pageCount
        );
        LastAsyncCommand = new AsyncRelayCommand(
            async () => await GetCustomerListAsync(_pageCount),
            () => _pageNumber != _pageCount
        );

        SearchRequestModel.Fields.Clear();
        string[] fields = { "title", "email", "address", "phone", "person" };
        SearchRequestModel.Fields.AddRange(fields);

        Refresh();
        //Task.Run(GetCustomerListAsync);
    }


    /// <summary>
    ///     The collection of customers in the list.
    /// </summary>
    public ObservableCollection<CustomerViewModel> Customers { get; } = new();


    /// <summary>
    ///     Gets or sets the selected customer, or null if no customer is selected.
    /// </summary>
    public CustomerViewModel SelectedCustomer
    {
        get => _selectedCustomer;
        set => SetProperty(ref _selectedCustomer, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        private set => SetProperty(ref _isLoading, value);
    }

    public IAsyncRelayCommand FirstAsyncCommand { get; }

    public IAsyncRelayCommand PreviousAsyncCommand { get; }

    public IAsyncRelayCommand NextAsyncCommand { get; }

    public IAsyncRelayCommand LastAsyncCommand { get; }

    public List<uint> PageSizes => new() { 5, 10, 20, 50, 100 };
    
    public List<SizeLimitModel> LimitSizeOptions => new()
    {
        new SizeLimitModel{Value =8000000000,Name = "8GB"},
        new SizeLimitModel{Value =16000000000,Name = "16GB"},
        new SizeLimitModel{Value =24000000000,Name = "24GB"},
        new SizeLimitModel{Value =32000000000,Name = "32GB"},
    };
    
    public uint PageSize
    {
        get => SearchRequestModel.PerPage;
        set
        {
            SearchRequestModel.PerPage = value;
            Refresh();
        }
    }

    public SearchRequestModel SearchRequestModel { get; } = new()
    {
        SortColumn = "title"
    };

    public int PageNumber
    {
        get => _pageNumber;
        private set => SetProperty(ref _pageNumber, value);
    }

    public int PageCount
    {
        get => _pageCount;
        private set => SetProperty(ref _pageCount, value);
    }

    /// <summary>
    ///     Gets the complete list of customers from the database.
    /// </summary>
    public async Task GetCustomerListAsync(int pageIndex)
    {
        await _dispatcherQueue.EnqueueAsync(() => IsLoading = true);

        SearchRequestModel.CurrentPage = (uint)pageIndex;

        var customers = await Ioc.Default.GetRequiredService<ISnpRepository>().Customers
            .SearchCustomerAsync(SearchRequestModel);
        if (customers.Items == null) return;

        PageNumber = customers.PageIndex;
        PageCount = customers.PageCount;
        FirstAsyncCommand.NotifyCanExecuteChanged();
        PreviousAsyncCommand.NotifyCanExecuteChanged();
        NextAsyncCommand.NotifyCanExecuteChanged();
        LastAsyncCommand.NotifyCanExecuteChanged();

        await _dispatcherQueue.EnqueueAsync(() =>
        {
            Customers.Clear();
            foreach (var c in customers.Items) Customers.Add(new CustomerViewModel(c));
            IsLoading = false;
        });
    }

    /// <summary>
    ///     Saves any modified customers and reloads the customer list from the database.
    /// </summary>
    public void Sync()
    {
        Task.Run(async () =>
        {
            foreach (var modifiedCustomer in Customers
                         .Where(customer => customer.IsModified).Select(customer => customer.Model))
                await Ioc.Default.GetRequiredService<ISnpRepository>().Customers.UpsertAsync(modifiedCustomer);

            Refresh();
            // await GetCustomerListAsync();
        });
    }

    private void Refresh()
    {
        SearchRequestModel.CurrentPage = 1;
        FirstAsyncCommand.Execute(null);
    }
}