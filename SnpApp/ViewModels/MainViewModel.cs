using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;
using CommunityToolkit.WinUI;

namespace Snp.App.ViewModels
{
    /// <summary>
    /// Provides data and commands accessible to the entire app.  
    /// </summary>
    public class MainViewModel : ObservableObject
    {
        private DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        
        private int _pageSize = 10;
        private int _pageNumber;
        private int _pageCount;

        /// <summary>
        /// Creates a new MainViewModel.
        /// </summary>
        
        public MainViewModel()
        {
            FirstAsyncCommand = new AsyncRelayCommand(
                async () => await GetCustomerListAsync(1, _pageSize),
                // async () => await GetMountains(1000, _pageSize), // Test for out of range.
                () => _pageNumber != 1
            );
            PreviousAsyncCommand = new AsyncRelayCommand(
                async () => await GetCustomerListAsync(_pageNumber - 1, _pageSize),
                () => _pageNumber > 1
            );
            NextAsyncCommand = new AsyncRelayCommand(
                async () => await GetCustomerListAsync(_pageNumber + 1, _pageSize),
                () => _pageNumber < _pageCount
            );
            LastAsyncCommand = new AsyncRelayCommand(
                async () => await GetCustomerListAsync(_pageCount, _pageSize),
                () => _pageNumber != _pageCount
            );

            Refresh();
            //Task.Run(GetCustomerListAsync);
        }
        

        /// <summary>
        /// The collection of customers in the list. 
        /// </summary>
        public ObservableCollection<CustomerViewModel> Customers { get; } = new ();

        private CustomerViewModel _selectedCustomer;
        
        

        /// <summary>
        /// Gets or sets the selected customer, or null if no customer is selected. 
        /// </summary>
        public CustomerViewModel SelectedCustomer
        {
            get => _selectedCustomer;
            set => SetProperty(ref _selectedCustomer, value);
        }

        private bool _isLoading;

        /// <summary>
        /// Gets or sets a value indicating whether the Customers list is currently being updated. 
        /// </summary>
        public bool IsLoading
        {
            get => _isLoading; 
            set => SetProperty(ref _isLoading, value);
        }
        
        public IAsyncRelayCommand FirstAsyncCommand { get; }

        public IAsyncRelayCommand PreviousAsyncCommand { get; }

        public IAsyncRelayCommand NextAsyncCommand { get; }

        public IAsyncRelayCommand LastAsyncCommand { get; }
        
        
        public async Task InitializeAsync()
        {
            
            
            await dispatcherQueue.EnqueueAsync(() => IsLoading = true);

            var customers = await App.Repository.Customers.SearchCustomerAsync();
            if (customers == null)
            {
                return;
            }
            
            await dispatcherQueue.EnqueueAsync(() =>
            {
                Customers.Clear();
                foreach (var c in customers)
                {
                    Customers.Add(new CustomerViewModel(c));
                }
                IsLoading = false;
            });
        }
        
        public List<int> PageSizes => new() { 5, 10, 20, 50, 100 };

        public int PageSize
        {
            get => _pageSize;
            set
            {
                SetProperty(ref _pageSize, value);
                Refresh();
            }
        }

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
        /// Gets the complete list of customers from the database.
        /// </summary>
        public async Task GetCustomerListAsync(int pageIndex, int pageSize)
        {
            await dispatcherQueue.EnqueueAsync(() => IsLoading = true);

            var customers = await App.Repository.Customers.SearchCustomerAsync();
            if (customers == null)
            {
                return;
            }

            await dispatcherQueue.EnqueueAsync(() =>
            {
                Customers.Clear();
                foreach (var c in customers)
                {
                    Customers.Add(new CustomerViewModel(c));
                }
                IsLoading = false;
            });
        }

        /// <summary>
        /// Saves any modified customers and reloads the customer list from the database.
        /// </summary>
        public void Sync()
        {
            Task.Run(async () =>
            {
                foreach (var modifiedCustomer in Customers
                    .Where(customer => customer.IsModified).Select(customer => customer.Model))
                {
                    await App.Repository.Customers.UpsertAsync(modifiedCustomer);
                }

                Refresh();
                // await GetCustomerListAsync();
            });
        }
        
        private void Refresh()
        {
            _pageNumber = 0;
            FirstAsyncCommand.Execute(null);
        }
    }
}
