using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Dispatching;
using CommunityToolkit.WinUI;
using Snp.Models;

namespace Snp.App.ViewModels
{
    /// <summary>
    /// Provides a bindable wrapper for the Site model class, encapsulating various services for access by the UI.
    /// </summary>
    public class SiteViewModel : ObservableObject
    {
        private DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        /// <summary>
        /// Initializes a new instance of the SiteViewModel class that wraps the specified Site object.
        /// </summary>
        /// <param name="model">The site to wrap.</param>
        public SiteViewModel(Site model)
        {
            Model = model;

            // Create an ObservableCollection to wrap Site.LineItems so we can track
            // product additions and deletions.
            LineItems = new ObservableCollection<LineItem>(Model.LineItems);
            LineItems.CollectionChanged += LineItems_Changed;

            NewLineItem = new LineItemViewModel();

            if (model.Customer == null)
            {
                Task.Run(() => LoadCustomer(Model.CustomerId));
            }
        }

        /// <summary>
        /// Creates an SiteViewModel that wraps an Site object created from the specified ID.
        /// </summary>
        public async static Task<SiteViewModel> CreateFromGuid(uint siteId) =>
            new (await GetSite(siteId));

        /// <summary>
        /// Gets the underlying Site object. 
        /// </summary>
        public Site Model { get; }

        /// <summary>
        /// Loads the customer with the specified ID. 
        /// </summary>
        private async void LoadCustomer(uint customerId)
        {
            var customer = await App.Repository.Customers.GetOneById(customerId);
            await dispatcherQueue.EnqueueAsync(() =>
            {
                Customer = customer;
            });
        }

        /// <summary>
        /// Returns the site with the specified ID.
        /// </summary>
        private static async Task<Site> GetSite(uint siteId) =>
            await App.Repository.Sites.GetOneById(siteId); 

        /// <summary>
        /// Gets a value that specifies whether the user can refresh the page.
        /// </summary>
        public bool CanRefresh => Model != null && !IsModified && IsExistingSite;

        /// <summary>
        /// Gets a value that specifies whether the user can revert changes. 
        /// </summary>
        public bool CanRevert => Model != null && IsModified && IsExistingSite;

        /// <summary>
        /// Gets or sets the site's ID.
        /// </summary>
        public uint Id
        {
            get => Model.Id; 
            set
            {
                if (Model.Id != value)
                {
                    Model.Id = value;
                    OnPropertyChanged();
                    IsModified = true;
                }
            }
        }

        bool _IsModified;

        /// <summary>
        /// Gets or sets a value that indicates whether the underlying model has been modified. 
        /// </summary>
        public bool IsModified
        {
            get => _IsModified; 
            set
            {
                if (value != _IsModified)
                {
                    // Only record changes after the site has loaded. 
                    if (IsLoaded)
                    {
                        _IsModified = value;
                        OnPropertyChanged();
                        OnPropertyChanged(nameof(CanRevert));
                    }
                }
            }
        }

        /// <summary>
        /// Gets a value that indicates whether this is an existing site.
        /// </summary>
        public bool IsExistingSite => !IsNewSite;

        /// <summary>
        /// Gets a value that indicates whether there is a backing site.
        /// </summary>
        public bool IsLoaded => Model != null && (IsNewSite || Model.Customer != null);

        /// <summary>
        /// Gets a value that indicates whether there is not a backing site.
        /// </summary>
        public bool IsNotLoaded => !IsLoaded;

        /// <summary>
        /// Gets a value that indicates whether this is a newly-created site.
        /// </summary>
        public bool IsNewSite =>  string.IsNullOrEmpty(Model.Code); 

        /// <summary>
        /// Gets or sets the invoice number for this site. 
        /// </summary>
        public int InvoiceNumber
        {
            get => Model.Volume;
            set
            {
                if (Model.Volume != value)
                {
                    Model.Volume = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsNewSite));
                    OnPropertyChanged(nameof(IsLoaded));
                    OnPropertyChanged(nameof(IsNotLoaded));
                    OnPropertyChanged(nameof(IsNewSite));
                    OnPropertyChanged(nameof(IsExistingSite));
                    IsModified = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets the customer for this site. This value is null
        /// unless you manually retrieve the customer (using CustomerId) and 
        /// set it. 
        /// </summary>
        public Customer Customer
        {
            get => Model.Customer;
            set
            {
                if (Model.Customer != value)
                {
                    var isLoadingOperation = Model.Customer == null &&
                        value != null && !IsNewSite;
                    Model.Customer = value;
                    OnPropertyChanged();
                    if (isLoadingOperation)
                    {
                        OnPropertyChanged(nameof(IsLoaded));
                        OnPropertyChanged(nameof(IsNotLoaded));
                    }
                    else
                    {
                        IsModified = true;
                    }
                }
            }
        }

        private ObservableCollection<LineItem> _lineItems;
        
        /// <summary>
        /// Gets the line items in this invoice. 
        /// </summary>
        public ObservableCollection<LineItem> LineItems
        {
            get => _lineItems; 
            set
            {
                if (_lineItems != value)
                {
                    if (value != null)
                    {
                        value.CollectionChanged += LineItems_Changed;
                    }

                    if (_lineItems != null)
                    {
                        _lineItems.CollectionChanged -= LineItems_Changed;
                    }
                    _lineItems = value;
                    OnPropertyChanged();
                    IsModified = true;
                }
            }
        }

        /// <summary>
        /// Notifies anyone listening to this object that a line item changed. 
        /// </summary>
        private void LineItems_Changed(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (LineItems != null)
            {
                Model.LineItems = LineItems.ToList();
            }

            OnPropertyChanged(nameof(LineItems));
            //OnPropertyChanged(nameof(Subtotal));
            //OnPropertyChanged(nameof(Tax));
           // OnPropertyChanged(nameof(GrandTotal));
            IsModified = true;
        }

        private LineItemViewModel _newLineItem;

        /// <summary>
        /// Gets or sets the line item that the user is currently working on.
        /// </summary>
        public LineItemViewModel NewLineItem
        {
            get => _newLineItem; 
            set
            {
                if (value != _newLineItem)
                {
                    if (value != null)
                    {
                        value.PropertyChanged += NewLineItem_PropertyChanged;
                    }

                    if (_newLineItem != null)
                    {
                        _newLineItem.PropertyChanged -= NewLineItem_PropertyChanged;
                    }

                    _newLineItem = value;
                    UpdateNewLineItemBindings();
                }
            }
        }

        private void NewLineItem_PropertyChanged(object sender, PropertyChangedEventArgs e) => UpdateNewLineItemBindings();

        private void UpdateNewLineItemBindings()
        {
            OnPropertyChanged(nameof(NewLineItem));
            OnPropertyChanged(nameof(HasNewLineItem));
            OnPropertyChanged(nameof(NewLineItemProductListPriceFormatted));
        }

        /// <summary>
        /// Gets or sets whether there is a new line item in progress.
        /// </summary>
        public bool HasNewLineItem => NewLineItem != null && NewLineItem.Product != null;

        /// <summary>
        /// Gets the product list price of the new line item, formatted for display.
        /// </summary>
        public string NewLineItemProductListPriceFormatted => (NewLineItem?.Product?.ListPrice ?? 0).ToString("c");

        // /// <summary>
        // /// Gets or sets the date this site was placed. 
        // /// </summary>
        // public DateTime DatePlaced
        // {
        //     get => Model.DatePlaced;
        //     set
        //     {
        //         if (Model.DatePlaced != value)
        //         {
        //             Model.DatePlaced = value;
        //             OnPropertyChanged();
        //             IsModified = true;
        //         }
        //     }
        // }
        //
        // /// <summary>
        // /// Gets or sets the date this site was filled. 
        // /// This value is automatically updated when the 
        // /// SiteStatus changes. 
        // /// </summary>
        // public DateTime? DateFilled
        // {
        //     get => Model.DateFilled;
        //     set
        //     {
        //         if (value != Model.DateFilled)
        //         {
        //             Model.DateFilled = value;
        //             OnPropertyChanged();
        //             IsModified = true;
        //         }
        //     }
        // }
        //
        // /// <summary>
        // /// Gets the subtotal. This value is calculated automatically. 
        // /// </summary>
        // public decimal Subtotal => Model.Subtotal;
        //
        // /// <summary>
        // /// Gets the tax. This value is calculated automatically. 
        // /// </summary>
        // public decimal Tax => Model.Tax;
        //
        // /// <summary>
        // /// Gets the total. This value is calculated automatically. 
        // /// </summary>
        // public decimal GrandTotal => Model.GrandTotal;

        /// <summary>
        /// Gets or sets the shipping address, which may be different
        /// from the customer's primary address. 
        /// </summary>
        public string Address
        {
            get => Model.Address; 
            set
            {
                if (Model.Address != value)
                {
                    Model.Address = value;
                    OnPropertyChanged();
                    IsModified = true;
                }
            }
        }

        // /// <summary>
        // /// Gets the set of payment status values so we can populate the payment status combo box.
        // /// </summary>
        // public List<string> PaymentStatusValues => Enum.GetNames(typeof(PaymentStatus)).ToList();
        //
        // /// <summary>
        // /// Sets the PaymentStatus property by parsing a string representation of the enum value.
        // /// </summary>
        // public void SetPaymentStatus(object value) => PaymentStatus = value == null ?
        //     PaymentStatus.Unpaid : (PaymentStatus)Enum.Parse(typeof(PaymentStatus), value as string);
        //
        // /// <summary>
        // /// Gets or sets the payment status.
        // /// </summary>
        // public PaymentStatus PaymentStatus
        // {
        //     get => Model.PaymentStatus;
        //     set
        //     {
        //         if (Model.PaymentStatus != value)
        //         {
        //             Model.PaymentStatus = value;
        //             OnPropertyChanged();
        //             IsModified = true;
        //         }
        //     }
        // }
        //
        // /// <summary>
        // /// Gets the set of payment term values, so we can populate the term combo box. 
        // /// </summary>
        // public List<string> TermValues => Enum.GetNames(typeof(Term)).ToList();
        //
        // /// <summary>
        // /// Sets the Term property by parsing a string representation of the enum value.
        // /// </summary>
        // public Term SetTerm(object value) => Term = value == null ?
        //     Term.Net1 : (Term)Enum.Parse(typeof(Term), value as string);
        //
        // /// <summary>
        // /// Gets or sets the payment term.
        // /// </summary>
        // public Term Term
        // {
        //     get => Model.Term;
        //     set
        //     {
        //         if (Model.Term != value)
        //         {
        //             Model.Term = value;
        //             OnPropertyChanged();
        //             IsModified = true;
        //         }
        //     }
        // }
        //
        // /// <summary>
        // /// Gets the set of site status values so we can populate the site status combo box. 
        // /// </summary>
        // public List<string> SiteStatusValues => Enum.GetNames(typeof(SiteStatus)).ToList();
        //
        // /// <summary>
        // /// Sets the Status property by parsing a string representation of the enum value.
        // /// </summary>
        // public SiteStatus SetSiteStatus(object value) => SiteStatus = value == null ? 
        //     SiteStatus.Open : (SiteStatus)Enum.Parse(typeof(SiteStatus), value as string);
        //
        // /// <summary>
        // /// Gets or sets the site status.
        // /// </summary>
        // public SiteStatus SiteStatus
        // {
        //     get => Model.Status;
        //     set
        //     {
        //         if (Model.Status != value)
        //         {
        //             Model.Status = value;
        //             OnPropertyChanged();
        //
        //             // Update the DateFilled value.
        //             DateFilled = Model.Status == SiteStatus.Filled ? (DateTime?)DateTime.Now : null;
        //             IsModified = true;
        //         }
        //     }
        // }

        /// <summary>
        /// Gets the name of the site's customer. 
        /// </summary>
        public string CustomerName => Model.Customer.Title;

        /// <summary>
        /// Saves the current site to the database. 
        /// </summary>
        public async Task SaveSiteAsync()
        {
            Site result = null;
            try
            {
                result = await App.Repository.Sites.UpsertAsync(Model);
            }
            catch (Exception ex)
            {
                throw new SiteSavingException("Unable to save. There might have been a problem " +
                    "connecting to the database. Please try again.", ex);
            }

            if (result != null)
            {
                await dispatcherQueue.EnqueueAsync(() => IsModified = false);
            }
            else
            {
                await dispatcherQueue.EnqueueAsync(() => new SiteSavingException(
                    "Unable to save. There might have been a problem " +
                    "connecting to the database. Please try again."));
            }
        }

        /// <summary>
        /// Stores the product suggestions. 
        /// </summary>
        public ObservableCollection<Product> ProductSuggestions { get; } = new ObservableCollection<Product>();

        /// <summary>
        /// Queries the database and updates the list of new product suggestions. 
        /// </summary>
        public async void UpdateProductSuggestions(string queryText)
        {
            ProductSuggestions.Clear();

            // if (!string.IsNullOrEmpty(queryText))
            // {
            //     var suggestions = await App.Repository.Products.GetAsync(queryText);
            //
            //     foreach (Product p in suggestions)
            //     {
            //         ProductSuggestions.Add(p);
            //     }
            // }
        }
    }
}
