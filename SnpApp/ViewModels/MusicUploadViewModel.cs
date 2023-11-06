using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;


namespace Snp.App.ViewModels
{
    public class MusicUploadViewModel : BindableBase
    {
        
        
        private DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        
        public MusicUploadViewModel(){}
        
        public async Task Add(Item item)
        {
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                if (!Files.Any(p =>p.Name==item.Name && p.Path == item.Path))
                {

                    // Add new object to the collection
                    Files.Add(item);
                }
            });
        }
        
        public async Task Upload()
        {
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                if (Files.Count > 0)
                {
                    foreach (var file in Files)
                    {
                        App.Repository.MusicUploads.Upload(file.Path,file.Name); 
                    }
                }
            });
        }
        
        public ObservableCollection<Item> Files { get; } = new ();
        
        public List<Item> Selected { get; set; } = new();
        
    }

    public class Item
    {
        public string Name { get; set; }
        public string Path { get; set; }
    }
}