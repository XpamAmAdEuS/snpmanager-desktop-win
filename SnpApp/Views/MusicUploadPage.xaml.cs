using System;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Navigation;
using Snp.App.Helper;
using Snp.Core.Services;
using Snp.App.ViewModels;
using WinRT.Interop;

namespace Snp.App.Views
{
    
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MusicUploadPage
    {
        
        private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        /// <summary>
        /// Initializes the page.
        /// </summary>
        public MusicUploadPage()
        {
            InitializeComponent();
            this.DataContext = Ioc.Default.GetService<MusicUploadViewModel>();
        }
        
        public MusicUploadViewModel ViewModel => (MusicUploadViewModel)DataContext;

        /// <summary>
        /// Resets the customer list when leaving the page.
        /// </summary>
        protected override async void OnNavigatedFrom(NavigationEventArgs e)
        {
            
        }

        /// <summary>
        /// Applies any existing filter when navigating to the page.
        /// </summary>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
           
        }
        
         private async void PickFolderButton_Click(object sender, RoutedEventArgs e)
        {
            
            // Create a folder picker
            FolderPicker openPicker = new Windows.Storage.Pickers.FolderPicker();

            // Retrieve the window handle (HWND) of the current WinUI 3 window.
            var window = WindowHelper.GetWindowForElement(this);
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

            // Initialize the folder picker with the window handle (HWND).
            InitializeWithWindow.Initialize(openPicker, hWnd);

            // Set options for your folder picker
            openPicker.SuggestedStartLocation = PickerLocationId.Desktop;
            openPicker.FileTypeFilter.Add("*");

            // Open the picker for the user to pick a folder
            StorageFolder folder = await openPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder);
                
                await ViewModel.AddStorageFolder(folder);
                
            }
        }
        
        private void StartUpload_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Upload();
        }
        
        private async void AddMusicFiles_Click(object sender, RoutedEventArgs e)
        {
            
            var files = await Ioc.Default.GetRequiredService<IFilePickManager>().PickMultipleFilesAsync();

            if (files is not { Count: > 0 }) return;
            foreach (var item in files)
            {
                ViewModel.AddStorageFile(item);
            }
        }
        
        private void Grid_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
        }
        
        private async void Grid_Drop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var items = await e.DataView.GetStorageItemsAsync();
                if (items.Count > 0)
                {
                    foreach (var item in items)
                    {
                        switch (item)
                        {
                            case StorageFolder folder:
                               // ViewModel.AddFolder(folder);
                                break;
                            case StorageFile file:
                               // ViewModel.AddFile(file);
                                break;
                        }
                    }
                }
            }
        }
    }
}
