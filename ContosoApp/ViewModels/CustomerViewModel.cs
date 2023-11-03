using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;
using Contoso.Models;


namespace Contoso.App.ViewModels
{
    /// <summary>
    /// Provides a bindable wrapper for the Customer model class, encapsulating various services for access by the UI.
    /// </summary>
    public class CustomerViewModel : BindableBase, IEditableObject
    {
        private DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        /// <summary>
        /// Initializes a new instance of the CustomerViewModel class that wraps a Customer object.
        /// </summary>
        public CustomerViewModel(Customer model = null) => Model = model ?? new Customer();

        private Customer _model;

        /// <summary>
        /// Gets or sets the underlying Customer object.
        /// </summary>
        public Customer Model
        {
            get => _model;
            set
            {
                if (_model != value)
                {
                    _model = value;

                    // Raise the PropertyChanged event for all properties.
                    OnPropertyChanged(string.Empty);
                }
            }
        }
        
        public uint Id => Model.Id;

        /// <summary>
        /// Gets or sets the customer's title.
        /// </summary>
        public string Title
        {
            get => Model.Title;
            set
            {
                if (value != Model.Title)
                {
                    Model.Title = value;
                    IsModified = true;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Email));
                }
            }
        }

        /// <summary>
        /// Gets or sets the customer's person.
        /// </summary>
        public string Person
        {
            get => Model.Person;
            set
            {
                if (value != Model.Person)
                {
                    Model.Person = value;
                    IsModified = true;
                    OnPropertyChanged();
                }
            }
        }
        

        /// <summary>
        /// Gets or sets the customer's address.
        /// </summary>
        public string Address
        {
            get => Model.Address;
            set
            {
                if (value != Model.Address)
                {
                    Model.Address = value;
                    IsModified = true;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the customer's SizeLimit.
        /// </summary>
        public uint SizeLimit
        {
            get => Model.SizeLimit;
            set
            {
                if (value != Model.SizeLimit)
                {
                    Model.SizeLimit = value;
                    IsModified = true;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the customer's phone number. 
        /// </summary>
        public string Phone
        {
            get => Model.Phone;
            set
            {
                if (value != Model.Phone)
                {
                    Model.Phone = value;
                    IsModified = true;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the customer's email. 
        /// </summary>
        public string Email
        {
            get => Model.Email;
            set
            {
                if (value != Model.Email)
                {
                    Model.Email = value;
                    IsModified = true;
                    OnPropertyChanged();
                }
            }
        }
        
        /// <summary>
        /// Gets or sets the customer's email. 
        /// </summary>
        public bool Muted
        {
            get => Model.Muted;
            set
            {
                if (value != Model.Muted)
                {
                    Model.Muted = value;
                    IsModified = true;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the underlying model has been modified. 
        /// </summary>
        /// <remarks>
        /// Used when sync'ing with the server to reduce load and only upload the models that have changed.
        /// </remarks>
        public bool IsModified { get; set; }
        

        private bool _isLoading;

        /// <summary>
        /// Gets or sets a value that indicates whether to show a progress bar. 
        /// </summary>
        public bool IsLoading
        {
            get => _isLoading;
            set => Set(ref _isLoading, value);
        }

        private bool _isNewCustomer;

        /// <summary>
        /// Gets or sets a value that indicates whether this is a new customer.
        /// </summary>
        public bool IsNewCustomer
        {
            get => _isNewCustomer;
            set => Set(ref _isNewCustomer, value);
        }

        private bool _isInEdit = false;

        /// <summary>
        /// Gets or sets a value that indicates whether the customer data is being edited.
        /// </summary>
        public bool IsInEdit
        {
            get => _isInEdit;
            set => Set(ref _isInEdit, value);
        }

        /// <summary>
        /// Saves customer data that has been edited.
        /// </summary>
        public async Task SaveAsync()
        {
            IsInEdit = false;
            IsModified = false;
            if (IsNewCustomer)
            {
                IsNewCustomer = false;
                App.ViewModel.Customers.Add(this);
            }

            await App.Repository.Customers.UpsertAsync(Model);
        }

        /// <summary>
        /// Raised when the user cancels the changes they've made to the customer data.
        /// </summary>
        public event EventHandler AddNewCustomerCanceled;

        /// <summary>
        /// Cancels any in progress edits.
        /// </summary>
        public async Task CancelEditsAsync()
        {
            if (IsNewCustomer)
            {
                AddNewCustomerCanceled?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                await RevertChangesAsync();
            }
        }

        /// <summary>
        /// Discards any edits that have been made, restoring the original values.
        /// </summary>
        public async Task RevertChangesAsync()
        {
            IsInEdit = false;
            if (IsModified)
            {
                await RefreshCustomerAsync();
                IsModified = false;
            }
        }

        /// <summary>
        /// Enables edit mode.
        /// </summary>
        public void StartEdit() => IsInEdit = true;

        /// <summary>
        /// Reloads all of the customer data.
        /// </summary>
        public async Task RefreshCustomerAsync()
        {
            Model = await App.Repository.Customers.GetOneById(Model.Id);
        }
        
        /// <summary>
        /// Called when a bound DataGrid control causes the customer to enter edit mode.
        /// </summary>
        public void BeginEdit()
        {
            // Not used.
        }

        /// <summary>
        /// Called when a bound DataGrid control cancels the edits that have been made to a customer.
        /// </summary>
        public async void CancelEdit() => await CancelEditsAsync();

        /// <summary>
        /// Called when a bound DataGrid control commits the edits that have been made to a customer.
        /// </summary>
        public async void EndEdit() => await SaveAsync();
    }
}