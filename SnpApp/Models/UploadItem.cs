﻿using System;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Google.Protobuf;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Snp.V1;

namespace SnpApp.Models;

public partial class UploadItem :  ObservableObject
{
    private static readonly ChannelBase ChannelBase = GrpcChannel.ForAddress(
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


    private readonly MusicUploadCrud.MusicUploadCrudClient _client = new (ChannelBase);
    
    [ObservableProperty]
    private string _name  = null!;
    [ObservableProperty]
    private string _id  = null!;
    [ObservableProperty]
    private string _path  = null!;
    [ObservableProperty]
    private double _progress;
    [ObservableProperty]
    private bool _showPaused;
    [ObservableProperty]
    private bool _showError;
    
    [RelayCommand]
    public async Task Upload()
    {
        
        Progress = 0;
        using var requestStream = _client.UploadMusic();
        const int chunkSize = 512000;
        
        byte[] fileData;
        await using (var stream = File.Open(Path, FileMode.Open))
        {
            fileData = new byte[stream.Length];
            var readAmount = stream.ReadAsync(fileData, 0, (int)stream.Length);
            await readAmount;
        }
            
        // var fileData = File.ReadAllBytes(Path);
        var totalChunks = (int)Math.Ceiling((double)fileData.Length / chunkSize);

        await requestStream.RequestStream.WriteAsync(new UploadRequest { FileName = Name});
        
        for (var chunkIndex = 0; chunkIndex < totalChunks; chunkIndex++)
        {
            var startIndex = chunkIndex * chunkSize;
            var chunkLength = Math.Min(chunkSize, fileData.Length - startIndex);
            var chunk = new byte[chunkLength];

           var percent = (fileData.Length /chunkLength) *100;
           Progress += percent; 
            Array.Copy(fileData, startIndex, chunk, 0, chunkLength);
            await requestStream.RequestStream.WriteAsync(new UploadRequest { ChunkData = ByteString.CopyFrom(chunk) });
        }

        await requestStream.RequestStream.CompleteAsync();

        try
        {
            var finalResponse = await requestStream.ResponseAsync;

            Id = finalResponse.Id;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}