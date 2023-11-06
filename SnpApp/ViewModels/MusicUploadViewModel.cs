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
                if (!_myModel.Any(p =>p.Name==item.Name && p.Path == item.Path))
                {

                    _myModel.Add(item);
                    
                    OnPropertyChanged("Model");
                }
            });
        }
        
        public async Task Upload()
        {
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                if (_myModel.Count > 0)
                {
                    foreach (var file in _myModel)
                    {
                        App.Repository.MusicUploads.Upload(file.Path,file.Name); 
                    }
                }
            });
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