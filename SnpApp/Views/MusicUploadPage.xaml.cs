using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Microsoft.Extensions.DependencyInjection;
using Windows.Storage;
using Windows.Storage.Pickers;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Navigation;
using Snp.Core.Models;
using Snp.Core.ViewModels;
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
        
        private void StartUpload_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Upload();
        }
        
        private async void AddMusicFiles_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker fileOpenPicker = new()
            {
                ViewMode = PickerViewMode.Thumbnail,
                FileTypeFilter = { ".mp3" },
            };
            
            
            nint windowHandle = WindowNative.GetWindowHandle(this);
            InitializeWithWindow.Initialize(fileOpenPicker, windowHandle);

            IReadOnlyList<StorageFile> items = await fileOpenPicker.PickMultipleFilesAsync();

            if (items.Count > 0)
            {
                foreach (var item in items)
                {
                    AddFileToModel(item);
                }
            }
        }
        
        private async void AddMusicFolder_Click(object sender, RoutedEventArgs e)
        {
            FolderPicker folderOpenPicker = new()
            {
                ViewMode = PickerViewMode.Thumbnail,
            };

            nint windowHandle = WindowNative.GetWindowHandle(this);
            InitializeWithWindow.Initialize(folderOpenPicker, windowHandle);

            StorageFolder folder = await folderOpenPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                AddFolderToModel(folder);
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
            var t = new UploadItem
            {
                Name = item.Name,
                Path = item.Path,
                ShowError = false,
                ShowPaused = true,
                Progress = 0
            };
            await ViewModel.Add(t);
        }
    }
}
