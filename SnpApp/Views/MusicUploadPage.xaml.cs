using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Navigation;
using Snp.App.ViewModels;

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
        }

        /// <summary>
        /// Gets the app-wide ViewModel instance.
        /// </summary>
        public MusicUploadViewModel ViewModel  => App.MusicUploadViewModel;

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
        
        private void AddMusic_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Upload();
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
                                AddFolderToModel(folder);
                                break;
                            case StorageFile file:
                                AddFileToModel(file);
                                break;
                        }
                    }
                }
            }
        }
        
        

        private async void AddFolderToModel(StorageFolder folder)
        {
           
                foreach (var item in await folder.GetFilesAsync())
                {
                   AddFileToModel(item);
                }
            
        }
        
        private async void AddFileToModel(StorageFile item)
        {
            var t = new Item
            {
                Name = item.Name,
                Path = item.Path
            };
            await ViewModel.Add(t);
        }
    }
}
