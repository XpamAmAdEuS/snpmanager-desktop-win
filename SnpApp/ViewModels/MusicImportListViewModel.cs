using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;
using SnpCore.Models;
using SnpCore.Services;

namespace SnpApp.ViewModels;

public class MusicImportListViewModel : ObservableObject
{
    private bool _isLoading;
    private int _pageCount;
    private int _pageNumber;

    private MediaPlayer _mediaPlayer;

    private MusicImportViewModel _selectedMusic;
    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

    public MusicImportListViewModel()
    {
        FirstAsyncCommand = new AsyncRelayCommand(
            async () => await GetListAsync(1),
            () => _pageNumber != 1
        );
        PreviousAsyncCommand = new AsyncRelayCommand(
            async () => await GetListAsync(_pageNumber - 1),
            () => _pageNumber > 1
        );
        NextAsyncCommand = new AsyncRelayCommand(
            async () => await GetListAsync(_pageNumber + 1),
            () => _pageNumber < _pageCount
        );
        LastAsyncCommand = new AsyncRelayCommand(
            async () => await GetListAsync(_pageCount),
            () => _pageNumber != _pageCount
        );

        SearchRequestModel.Fields.Clear();
        string[] fields = { "title", "email", "address", "phone", "person" };
        SearchRequestModel.Fields.AddRange(fields);

        Refresh();
    }


  
    public ObservableCollection<MusicImportViewModel> Musics { get; } = new();

    
    public MusicImportViewModel SelectedMusic
    {
        get => _selectedMusic;
        set => SetProperty(ref _selectedMusic, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        private set => SetProperty(ref _isLoading, value);
    }

    public IAsyncRelayCommand FirstAsyncCommand { get; }

    public IAsyncRelayCommand PreviousAsyncCommand { get; }

    public IAsyncRelayCommand NextAsyncCommand { get; }

    public IAsyncRelayCommand LastAsyncCommand { get; }

    public List<uint> PageSizes => new() { 5, 10, 20, 50, 100 };
    
    public uint PageSize
    {
        get => SearchRequestModel.PerPage;
        set
        {
            SearchRequestModel.PerPage = value;
            Refresh();
        }
    }

    public SearchRequestModel SearchRequestModel { get; } = new()
    {
        SortColumn = "title"
    };

    public int PageNumber
    {
        get => _pageNumber;
        private set => SetProperty(ref _pageNumber, value);
    }

    public int PageCount
    {
        get => _pageCount;
        private set => SetProperty(ref _pageCount, value);
    }

    public void Play(MusicImport e)
    {
        _mediaPlayer.Source = new MediaPlaybackItem(MediaSource.CreateFromUri(new Uri($"http://192.168.1.36:50051/v1/music/import/file/{e.Hash}.mp3")));
        _mediaPlayer.Play();
        IsLoading = false;
    }

    public async Task GetListAsync(int pageIndex)
    {
        await _dispatcherQueue.EnqueueAsync(() => IsLoading = true);

        SearchRequestModel.CurrentPage = (uint)pageIndex;

        try
        {
            var result = await Ioc.Default.GetRequiredService<MusicImportService>()
                .SearchImportMusicAsync(SearchRequestModel);
            if (result.Items == null) return;

            PageNumber = result.PageIndex;
            PageCount = result.PageCount;
            FirstAsyncCommand.NotifyCanExecuteChanged();
            PreviousAsyncCommand.NotifyCanExecuteChanged();
            NextAsyncCommand.NotifyCanExecuteChanged();
            LastAsyncCommand.NotifyCanExecuteChanged();
            

            _mediaPlayer = new MediaPlayer();
            
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                Musics.Clear();
                
                foreach (var e in result.Items)
                {
                    Musics.Add(new MusicImportViewModel(e));
                   
                }
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            IsLoading = false;
            throw;
        }
        finally
        {
            IsLoading = false;
        }

        
    }

    /// <summary>
    ///     Saves any modified customers and reloads the customer list from the database.
    /// </summary>
    public void Sync()
    {
        Task.Run(async () =>
        {
            foreach (var modifiedMusic in Musics
                         .Where(entity => entity.IsModified).Select(entity => entity.Model))
                await Ioc.Default.GetRequiredService<MusicImportService>().UpsertAsync(modifiedMusic);

            Refresh();
            // await GetCustomerListAsync();
        });
    }

    private void Refresh()
    {
        SearchRequestModel.CurrentPage = 1;
        FirstAsyncCommand.Execute(null);
    }
}