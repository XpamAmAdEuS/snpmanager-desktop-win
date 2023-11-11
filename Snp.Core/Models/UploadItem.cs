using System;
using System.IO;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Google.Protobuf;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Snp.Core.Repository;
using Snp.V1;

namespace Snp.Core.Models;

public class UploadItem :  ObservableObject,ICommand
{
    
    private string _name;
    private string _id;
    private string _path;
    private double _progress;
    private bool _showPaused;
    private bool _showError;
    
    public string Name{
        get => _name;
        set => SetProperty(ref _name, value);
    }
    
    public string Id{
        get => _id;
        set
        {
            SetProperty(ref _id, value);
            OnPropertyChanged(nameof(Progress));
            OnPropertyChanged();
        } 
    }
    
    public string Path{
        get => _path;
        set => SetProperty(ref _path, value);
    }
    
    public double Progress{
        get => _progress;
        set => SetProperty(ref _progress, value);
    }
    private void downloadprogress(double value)  
    {  
        Progress += value;  
        OnPropertyChanged(nameof(Progress));  
    }  
    
    public bool ShowPaused{
        get => _showPaused;
        set => SetProperty(ref _showPaused, value);
    }
    public bool ShowError{
        get => _showError;
        set => SetProperty(ref _showError, value);
    }

    public bool CanExecute(object parameter) => true;

    
    public void Execute(object parameter)
    {
     
        ChannelBase channel = GrpcChannel.ForAddress(
            Constants.GrpcUrl,
            new GrpcChannelOptions
            {
                Credentials = ChannelCredentials.Insecure,
                LoggerFactory = LoggerFactory.Create(logging =>
                {
                    logging.AddConsole();
                    logging.AddDebug();
                    logging.SetMinimumLevel(LogLevel.Debug);
                })
            });
        
        
        MusicUploadCrud.MusicUploadCrudClient client = new MusicUploadCrud.MusicUploadCrudClient(channel);
        
        downloadprogress(0);
        using var requestStream = client.UploadMusic();
        const int chunkSize = 512000;
            
        byte[] fileData = File.ReadAllBytes(Path);
        int totalChunks = (int)Math.Ceiling((double)fileData.Length / chunkSize);

        for (int chunkIndex = 0; chunkIndex < totalChunks; chunkIndex++)
        {
            int startIndex = chunkIndex * chunkSize;
            int chunkLength = Math.Min(chunkSize, fileData.Length - startIndex);
            byte[] chunk = new byte[chunkLength];

            var percent = (fileData.Length /chunkLength) *100;
            downloadprogress(percent);
            
            Array.Copy(fileData, startIndex, chunk, 0, chunkLength);
            requestStream.RequestStream.WriteAsync(new UploadRequest { FileName = Name,ChunkData = ByteString.CopyFrom(chunk) });
        }

        requestStream.RequestStream.CompleteAsync();

        var finalResponse =  requestStream.ResponseAsync;

        Id = finalResponse.Id.ToString();


    }

    public event EventHandler CanExecuteChanged;
}