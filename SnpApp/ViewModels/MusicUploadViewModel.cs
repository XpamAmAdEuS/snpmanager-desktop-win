using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.UI.Dispatching;


namespace Snp.App.ViewModels
{
    public class MusicUploadViewModel : BindableBase
    {
        private DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        
        public MusicUploadViewModel()
        {
            
        }
        
        
        public ObservableCollection<string> Files { get; set; } = new ();
        
        public List<string> Selected { get; set; } = new();
    }
}