using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Windows.ApplicationModel.Email;
using CommunityToolkit.Mvvm.DependencyInjection;
using Snp.Core.Models;
using Snp.Core.Services;
using Snp.Core.ViewModels;

namespace Snp.App.Views
{
    /// <summary>
    /// Displays and edits an order.
    /// </summary>
    public sealed partial class SiteDetailPage : INotifyPropertyChanged
    {
        
        private readonly IDialogService DialogService;
        
        public SiteDetailPage(IDialogService dialogService)
        {
            this.InitializeComponent();
            this.DataContext = Ioc.Default.GetService<SiteViewModel>();
            DialogService = dialogService;
        }
        
        // public SiteViewModel ViewModel => (SiteViewModel)DataContext;

        // /// <summary>
        // /// Stores the view model. 
        // /// </summary>
        // private SiteViewModel _viewModel;
        //
        // /// <summary>
        // /// We use this object to bind the UI to our data.
        // /// </summary>
        public SiteViewModel ViewModel
        {
            get => (SiteViewModel)DataContext;
            set
            {
                if ((SiteViewModel)DataContext != value)
                {
                    DataContext = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Loads the specified order, a cached order, or creates a new order.
        /// </summary>
        /// <param name="e">Info about the event.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var id = (uint)e.Parameter;
            // var customer = App.ViewModel.Customers.Where(customer => customer.Model.Id == id).FirstOrDefault();
            //
            // if (customer != null)
            // {
            //     // Site is a new site
            //     ViewModel = new SiteViewModel(new Site(customer.Model));
            // }
            // else
            // {
            //     // Site is an existing site.
            //     var site = await App.Repository.Sites.GetOneById(id);
            //     ViewModel = new SiteViewModel(site);
            // }

            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// Check whether there are unsaved changes and warn the user.
        /// </summary>
        protected async override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (ViewModel.IsModified)
            {
                var saveDialog = new SaveChangesDialog()
                {
                    Title = $"Save changes to Invoice # {ViewModel.InvoiceNumber.ToString()}?",
                    Content = $"Invoice # {ViewModel.InvoiceNumber.ToString()} " + 
                        "has unsaved changes that will be lost. Do you want to save your changes?"
                };
                saveDialog.XamlRoot = this.Content.XamlRoot;
                await saveDialog.ShowAsync();
                SaveChangesDialogResult result = saveDialog.Result;

                switch (result)
                {
                    case SaveChangesDialogResult.Save:
                        await ViewModel.SaveSiteAsync();
                        break;
                    case SaveChangesDialogResult.DontSave:
                        break;
                    case SaveChangesDialogResult.Cancel:
                        if (e.NavigationMode == NavigationMode.Back)
                        {
                            Frame.GoForward();
                        }
                        else
                        {
                            Frame.GoBack();
                        }
                        e.Cancel = true;

                        // This flag gets cleared on navigation, so restore it. 
                        ViewModel.IsModified = true; 
                        break;
                }
            }

            base.OnNavigatingFrom(e);
        }

        /// <summary>
        /// Creates an email to the current customer.
        /// </summary>
        private async void emailButton_Click(object sender, RoutedEventArgs e)
        {
            var emailMessage = new EmailMessage
            {
                Body = $"Dear {ViewModel.CustomerName},",
                Subject = "A message from Contoso about order " +
                    $"#{ViewModel.InvoiceNumber} placed on "
            };

            if (!string.IsNullOrEmpty(ViewModel.Customer.Email))
            {
                var emailRecipient = new EmailRecipient(ViewModel.Customer.Email);
                emailMessage.To.Add(emailRecipient);
            }

            await EmailManager.ShowComposeNewEmailAsync(emailMessage);
        }

        /// <summary>
        /// Reloads the order.
        /// </summary>
        private async void RefreshButton_Click(object sender, RoutedEventArgs e) => 
            ViewModel = await SiteViewModel.CreateFromGuid(ViewModel.Id);

        /// <summary>
        /// Reverts the page.
        /// </summary>
        private async void RevertButton_Click(object sender, RoutedEventArgs e)
        {
            var saveDialog = new SaveChangesDialog()
            {
                Title = $"Save changes to Invoice # {ViewModel.InvoiceNumber.ToString()}?",
                Content = $"Invoice # {ViewModel.InvoiceNumber.ToString()} " + 
                    "has unsaved changes that will be lost. Do you want to save your changes?"
            };
            saveDialog.XamlRoot = this.Content.XamlRoot;
            await saveDialog.ShowAsync();
            SaveChangesDialogResult result = saveDialog.Result;

            switch (result)
            {
                case SaveChangesDialogResult.Save:
                    await ViewModel.SaveSiteAsync();
                    ViewModel = await SiteViewModel.CreateFromGuid(ViewModel.Id);
                    break;
                case SaveChangesDialogResult.DontSave:
                    ViewModel = await SiteViewModel.CreateFromGuid(ViewModel.Id);
                    break;
                case SaveChangesDialogResult.Cancel:
                    break;
            }         
        }

        /// <summary>
        /// Saves the current order.
        /// </summary>
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
                await ViewModel.SaveSiteAsync();
                Bindings.Update();
            }
            catch (SiteSavingException ex)
            {
                
                _ = DialogService.ShowMessageDialogAsync("Unable to save", $"There was an error saving your order:\n{ex.Message}");
            }
        }
        
        /// <summary>
        /// Fired when a property value changes. 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notifies listeners that a property value changed. 
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
