using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;
using Snp.Core.Models;


namespace Snp.App.ViewModels
{
    public partial class MusicUploadViewModel : ObservableObject
    {
        
        private DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        
        public MusicUploadViewModel ()
        {
            _myModel = new ObservableCollection<UploadItem> ();
            
            OnPropertyChanged();
        }
        
        public async Task Add(UploadItem item)
        {
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                if (!_myModel.Any(p =>
                        p.Name==item.Name && 
                        p.Path == item.Path)) {

                    _myModel.Add(item);
                    
                    OnPropertyChanged("Model");
                }
            });
        }
        
        public async Task Remove(UploadItem item)
        {
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                if (!_myModel.Any(p =>
                        p.Name==item.Name && 
                        p.Path == item.Path)) {

                    _myModel.Remove(item);
                    
                    OnPropertyChanged("Model");
                }
            });
        }
        
        [RelayCommand(CanExecute = nameof(CanClear))]
        public void Clear() => _myModel.Clear();

       
        
        
        private bool CanClear()
        {
            return true;
        }
        
        
        private bool _isUpLoading;

        /// <summary>
        /// Gets or sets a value indicating whether the Customers list is currently being updated. 
        /// </summary>
        public bool IsUpLoading
        {
            get => _isUpLoading; 
            set => SetProperty(ref _isUpLoading, value);
        }
        
        public async void Upload()
        {
            await _dispatcherQueue.EnqueueAsync(() => IsUpLoading = true);
            
            
            if (_myModel.Count > 0)
            {
                foreach (var t in _myModel)
                {
                    await _dispatcherQueue.EnqueueAsync(() => t.Upload());
                }

                IsUpLoading = false;
            }
        }
        
        private ObservableCollection<UploadItem> _myModel;
        
        public ObservableCollection<UploadItem> Model { get { return _myModel; } }
        
        public async Task AddStorageFolder(StorageFolder folder)
        {
           
            foreach (var item in await folder.GetFilesAsync())
            {
                AddStorageFile(item);
            }
            
        }
        
        public void AddStorageFile(StorageFile item)
        {
            var t = new UploadItem
            {
                Name = item.Name,
                Path = item.Path,
                ShowError = false,
                ShowPaused = true
            };
            Model.Add(t);
        }
    }
}