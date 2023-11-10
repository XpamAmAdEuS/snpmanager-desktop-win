using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI;
using Google.Protobuf;
using Microsoft.UI.Dispatching;
using Snp.Core.Models;
using Snp.V1;


namespace Snp.Core.ViewModels
{
    public class MusicUploadViewModel : ObservableObject
    {
        
        private DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        
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
                    
                        // using var requestStream = _client.UploadMusic();
                        // const int chunkSize = 512000;
                        // byte[] fileData;
                        // Task<int> readAmount;
                        // await using (var stream = File.Open(t.Path, FileMode.Open))
                        // {
                        //     fileData = new byte[stream.Length];
                        //     readAmount = stream.ReadAsync(fileData, 0, (int)stream.Length);
                        //     await readAmount;
                        // }
                        //
                        // // byte[] fileData = File.ReadAllBytes(path);
                        // int totalChunks = (int)Math.Ceiling((double)fileData.Length / chunkSize);
                        //
                        // for (int chunkIndex = 0; chunkIndex < totalChunks; chunkIndex++)
                        // {
                        //     int startIndex = chunkIndex * chunkSize;
                        //     int chunkLength = Math.Min(chunkSize, fileData.Length - startIndex);
                        //     byte[] chunk = new byte[chunkLength];
                        //     Array.Copy(fileData, startIndex, chunk, 0, chunkLength);
                        //     await requestStream.RequestStream.WriteAsync(new UploadRequest { FileName = t.Name,ChunkData = ByteString.CopyFrom(chunk) });
                        // }
                        //
                        // await requestStream.RequestStream.CompleteAsync();
                        //
                        // var finalResponse = await requestStream.ResponseAsync;
                        //
                        // t.Progress = 100;
                        // t.ShowPaused = false;
                  
                    
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
        
        private ObservableCollection<UploadItem> _myModel;

        public ObservableCollection<UploadItem> Model { get { return _myModel; } }

        public MusicUploadViewModel ()
        {
            _myModel = new ObservableCollection<UploadItem> ();
            
            OnPropertyChanged();
        }
    }
}