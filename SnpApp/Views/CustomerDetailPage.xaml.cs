using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using SnpApp.ViewModels;
using CommunityToolkit.WinUI.UI.Controls;
using SnpApp.Models;
using SnpApp.Navigation;
using SnpApp.Services;

namespace SnpApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CustomerDetailPage: INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes the page.
        /// </summary>
        public CustomerDetailPage()
        {
            InitializeComponent();
        }
        
        private CustomerViewModel? _viewModel;

        /// <summary>
        /// Used to bind the UI to the data.
        /// </summary>
        public CustomerViewModel? ViewModel
        {
            get => _viewModel;
            set
            {
                if (_viewModel != value)
                {
                    _viewModel = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Navigate to the previous page when the user cancels the creation of a new customer record.
        /// </summary>
        private void AddNewCustomerCanceled(object? sender, EventArgs e) => Frame.GoBack();

        /// <summary>
        /// Displays the selected customer data.
        /// </summary>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            
            
            
            if (e.Parameter == null)
            {
                ViewModel = new CustomerViewModel
                {
                    IsNewCustomer = true,
                    IsInEdit = true
                };
                VisualStateManager.GoToState(this, "NewCustomer", false);
            }
            else
            {
                
                var args = (NavigationRootPageArgs)e.Parameter;
                if (args.Parameter != null)
                {
                    var id = (uint)args.Parameter;
                    var c = await Ioc.Default.GetRequiredService<CustomerService>().GetOneById(id);
                    ViewModel = new CustomerViewModel(c);
                }
            }

            if (ViewModel != null) ViewModel.AddNewCustomerCanceled += AddNewCustomerCanceled;
            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// Check whether there are unsaved changes and warn the user.
        /// </summary>
        protected override async void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (ViewModel is { IsModified: true })
            {
                // Cancel the navigation immediately, otherwise it will continue at the await call. 
                e.Cancel = true;

                void ResumeNavigation()
                {
                    if (e.NavigationMode == NavigationMode.Back)
                    {
                        Frame.GoBack();
                    }
                    else
                    {
                        Frame.Navigate(e.SourcePageType, e.Parameter, e.NavigationTransitionInfo);
                    }
                }

                var saveDialog = new SaveChangesDialog
                {
                    Title = "Save changes?",
                    XamlRoot = Content.XamlRoot
                };
                await saveDialog.ShowAsync();
                var result = saveDialog.Result;

                switch (result)
                {
                    case SaveChangesDialogResult.Save:
                        await ViewModel.SaveAsync();
                        ResumeNavigation();
                        break;
                    case SaveChangesDialogResult.DontSave:
                        await ViewModel.RevertChangesAsync();
                        ResumeNavigation();
                        break;
                    case SaveChangesDialogResult.Cancel:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            base.OnNavigatingFrom(e);
        }

        /// <summary>
        /// Disconnects the AddNewCustomerCanceled event handler from the ViewModel 
        /// when the parent frame navigates to a different page.
        /// </summary>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (ViewModel != null) ViewModel.AddNewCustomerCanceled -= AddNewCustomerCanceled;
            base.OnNavigatedFrom(e);
        }

        /// <summary>
        /// Initializes the AutoSuggestBox portion of the search box.
        /// </summary>
        private void CustomerSearchBox_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        /// <summary>
        /// Queries the database for a customer result matching the search text entered.
        /// </summary>
        private void CustomerSearchBox_TextChanged(AutoSuggestBox sender,
            AutoSuggestBoxTextChangedEventArgs args)
        {
            // We only want to get results when it was a user typing,
            // otherwise we assume the value got filled in by TextMemberPath
            // or the handler for SuggestionChosen
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                // If no search query is entered, refresh the complete list.
                if (string.IsNullOrEmpty(sender.Text))
                {
                    sender.ItemsSource = null;
                }
                // else
                // {
                //     var repo = Ioc.Default.GetService<CustomerService>();
                //     if (repo?.Customers != null)
                //         sender.ItemsSource = await repo.Customers.SearchByTitleAsync(sender.Text);
                // }
            }
        }

        /// <summary>
        /// Search by customer name, email, or phone number, then display results.
        /// </summary>
        private void CustomerSearchBox_QuerySubmitted(AutoSuggestBox sender,
            AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion is Customer customer)
            {
                Frame.Navigate(typeof(CustomerDetailPage), customer.Id);
            }
        }

      
        
        private void ViewSiteButton_Click(object sender, RoutedEventArgs e) =>
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            Frame.Navigate(typeof(SiteDetailPage), ((sender as FrameworkElement).DataContext as Site).Id,
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                new DrillInNavigationTransitionInfo());

        /// <summary>
        /// Adds a new order for the customer.
        /// </summary>
        private void AddSite_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null) Frame.Navigate(typeof(SiteDetailPage), ViewModel.Model.Id);
        }



        /// <summary>
        /// Sorts the data in the DataGrid.
        /// </summary>
        private void DataGrid_Sorting(object sender, DataGridColumnEventArgs e)
        {
            if (ViewModel != null) ((DataGrid)sender).Sort(e.Column, ViewModel.Sites.Sort);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
