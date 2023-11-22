using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;
using SnpApp.Models;
using SnpApp.Services;

namespace SnpApp.ViewModels;

public class MusicListViewModel : ObservableObject
{
    private bool _isLoading;
    private int _pageCount;
    private int _pageNumber;
    
    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

    public MusicListViewModel()
    {
        FirstAsyncCommand = new AsyncRelayCommand(
            async () => await GetMusicListAsync(1),
            () => _pageNumber != 1
        );
        PreviousAsyncCommand = new AsyncRelayCommand(
            async () => await GetMusicListAsync(_pageNumber - 1),
            () => _pageNumber > 1
        );
        NextAsyncCommand = new AsyncRelayCommand(
            async () => await GetMusicListAsync(_pageNumber + 1),
            () => _pageNumber < _pageCount
        );
        LastAsyncCommand = new AsyncRelayCommand(
            async () => await GetMusicListAsync(_pageCount),
            () => _pageNumber != _pageCount
        );

        SearchRequestModel.Fields.Clear();
        string[] fields = { "title", "email", "address", "phone", "person" };
        SearchRequestModel.Fields.AddRange(fields);

        Refresh();
        //Task.Run(GetMusicListAsync);
    }


    /// <summary>
    ///     The collection of musics in the list.
    /// </summary>
    public ObservableCollection<Music> Musics { get; } = new();

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
    
    public List<SizeLimitModel> LimitSizeOptions => new()
    {
        new SizeLimitModel{Value =8000000000,Name = "8GB"},
        new SizeLimitModel{Value =16000000000,Name = "16GB"},
        new SizeLimitModel{Value =24000000000,Name = "24GB"},
        new SizeLimitModel{Value =32000000000,Name = "32GB"},
    };
    
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

    public async Task GetMusicListAsync(int pageIndex)
    {
        await _dispatcherQueue.EnqueueAsync(() => IsLoading = true);

        SearchRequestModel.CurrentPage = (uint)pageIndex;

        try
        {
            var musics = await Ioc.Default.GetRequiredService<MusicService>()
                .SearchMusicAsync(SearchRequestModel);
            if (musics.Items == null) return;

            PageNumber = musics.PageIndex;
            PageCount = musics.PageCount;
            FirstAsyncCommand.NotifyCanExecuteChanged();
            PreviousAsyncCommand.NotifyCanExecuteChanged();
            NextAsyncCommand.NotifyCanExecuteChanged();
            LastAsyncCommand.NotifyCanExecuteChanged();

            await _dispatcherQueue.EnqueueAsync(() =>
            {
                Musics.Clear();
                foreach (var c in musics.Items) Musics.Add(c);
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
    
    private void Refresh()
    {
        SearchRequestModel.CurrentPage = 1;
        FirstAsyncCommand.Execute(null);
    }
}