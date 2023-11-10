using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Dispatching;
using Snp.Core.Models;
using Snp.Core.Repository;
using CommunityToolkit.WinUI;

namespace Snp.Core.ViewModels
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
            
            var repo = Ioc.Default.GetService<ISnpRepository>();
            
            var customer = await repo.Customers.GetOneById(customerId);
            await dispatcherQueue.EnqueueAsync(() =>
            {
                Customer = customer;
            });
        }

        /// <summary>
        /// Returns the site with the specified ID.
        /// </summary>
        private static async Task<Site> GetSite(uint siteId) =>
            await Ioc.Default.GetService<ISnpRepository>().Sites.GetOneById(siteId); 

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
                result = await Ioc.Default.GetService<ISnpRepository>().Sites.UpsertAsync(Model);
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
    }
}
