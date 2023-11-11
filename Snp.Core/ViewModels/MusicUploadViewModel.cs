using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI;
using Google.Protobuf;
using Microsoft.UI.Dispatching;
using Snp.Core.Models;
using Snp.Core.Services;
using Snp.V1;


namespace Snp.Core.ViewModels
{
    public class MusicUploadViewModel : ObservableObject
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
        
        public async Task Clear()
        {
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                _myModel.Clear();
                OnPropertyChanged("Model");
            });
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
                    await _dispatcherQueue.EnqueueAsync(() => t.Execute(null));
                }

                IsUpLoading = false;
            }
        }
        
        private ObservableCollection<UploadItem> _myModel;

        public ObservableCollection<UploadItem> Model { get { return _myModel; } }
    }
}