using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Snp.App.Models;


namespace Snp.App.ViewModels
{
    public class MusicUploadViewModel : BindableBase
    {
        
        
        private DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        
        public async Task Add(UploadItemModel item)
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
        
        private bool _isUpLoading;

        /// <summary>
        /// Gets or sets a value indicating whether the Customers list is currently being updated. 
        /// </summary>
        public bool IsUpLoading
        {
            get => _isUpLoading; 
            set => Set(ref _isUpLoading, value);
        }
        
        public async Task Upload()
        {
            await _dispatcherQueue.EnqueueAsync(() => IsUpLoading = true);
            
            
            if (_myModel.Count > 0)
            {
                foreach (var t in _myModel)
                {
                    await _dispatcherQueue.EnqueueAsync(() =>
                    {
                        App.Repository.MusicUploads.Upload(t.Path,t.Name);
                        t.Progress = 100;
                        t.ShowPaused = false;
                        OnPropertyChanged(nameof(Model));
                    });
                    
                   // await App.Repository.MusicUploads.Upload(t.Path,t.Name);
                   //  t.Progress = 100;
                   //  t.ShowPaused = false;
                }

                IsUpLoading = false;
            }
            
            
            // await _dispatcherQueue.EnqueueAsync(() =>
            // {
            //     if (_myModel.Count > 0)
            //     {
            //         foreach (var file in _myModel)
            //         {
            //             
            //          App.Repository.MusicUploads.Upload(file.Path,file.Name);
            //
            //          
            //          
            //              var found = _myModel.FirstOrDefault(p=>p.Name==file.Name && 
            //                                                     p.Path == file.Path);
            //              found.Progress = 100;
            //              found.ShowPaused = false;
            //             
            //           
            //         }
            //         IsUpLoading = false;
            //     }
            // });
        }
        
        private ObservableCollection<UploadItemModel> _myModel;

        public ObservableCollection<UploadItemModel> Model { get { return _myModel; } }

        private IList _selectedModels = new ArrayList ();

        public IList TestSelected
        {
            get { return _selectedModels; }
            set
            {
                _selectedModels = value;
                OnPropertyChanged("TestSelected");
            }
        }

        public MusicUploadViewModel ()
        {
            _myModel = new ObservableCollection<UploadItemModel> ();
            OnPropertyChanged("Model");
        }
    }
}