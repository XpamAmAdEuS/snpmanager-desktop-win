using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Interop;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;
using SnpApp.Models;
using SnpApp.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace SnpApp.ViewModels;

public class MusicImportListViewModel : ObservableObject
{
    private bool _isLoading;
    private int _pageCount;
    private int _pageNumber;
    private bool _isProcessing;
    
    [Reactive]
    public bool? AllStatesIsChecked { get; set; }
    
    [Reactive]
    public bool AllStatesIsSelected { get; set; }
    
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
        
        CheckCommand = new RelayCommand(OnCheck);
        CheckAllCommand = new RelayCommand(OnCheckAll);
        IsAllSelected = false;

        SearchRequestModel.Fields.Clear();
        string[] fields = { "title", "email", "address", "phone", "person" };
        SearchRequestModel.Fields.AddRange(fields);
        // Entries = new ();
        
        AllStatesIsChecked = false;

        
        this.WhenAnyValue(x => x.AllStatesIsChecked).Subscribe(x =>
        {
            if (x != null)
            {
                UpdateCheckBoxes(x);
            }
        });
        this.WhenAnyValue(x => x.AllStatesIsSelected).Subscribe(x =>
        {
            if ((AllStatesIsChecked == null || AllStatesIsChecked == false) && x == true)
            {
                AllStatesIsChecked = true;
            }
            else if (AllStatesIsChecked == true && x == false)
            {
                AllStatesIsChecked = false;
            }
        });
        
        
        Refresh();
    }
    
    private void OnSelectedStatesChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        UpdateAllStates();
    }

    private void UpdateCheckBoxes(bool? isChecked)
    {
        if (!_isProcessing)
        {
            _isProcessing = true;

            if (isChecked == true)
            {
                foreach (var state in Entries)
                {
                    if (state.IsChecked == false) state.IsChecked = true;
                }
            }
            else if (isChecked == false)
            {
                foreach (var state in Entries)
                {
                    if (state.IsChecked == true) state.IsChecked = false;
                }
            }

            _isProcessing = false;
        }
    }

    private void UpdateAllStates()
    {
        var selectedStatesCount = SelectedStates.Count;
        Debug.WriteLine($"CheckBoxes: {selectedStatesCount}/{Entries.Count}");

        if (selectedStatesCount == Entries.Count)
        {
            AllStatesIsSelected = true;
            AllStatesIsChecked = true;
        }
        else if (selectedStatesCount == 0)
        {
            AllStatesIsSelected = false;
            AllStatesIsChecked = false;
        }
        else
        {
            AllStatesIsChecked = null;
        }
    }
    
    private void OnCheckAll()
    {
        // if (IsAllSelected == true)
        //     Entries.ForEach(x => x.IsChecked = true);
        // else
        //     Entries.ForEach(x => x.IsChecked = false);
    }

    private void OnCheck()
    {
        if (Entries.All(x => x.IsChecked))
            IsAllSelected = true;
        else if (Entries.All(x => !x.IsChecked))
            IsAllSelected = false;
        else
            IsAllSelected = null;
    }
    
    
    private ObservableCollection<MusicImport> _entries = new ();
    
    // public ObservableCollection<MusicImport> Entries { get; } = new();
    
    public ObservableCollection<MusicImport> SelectedStates { get; set; }
    
    public ObservableCollection<MusicImport> Entries
    {
        get => _entries;
        set
        {
            if (Equals(value, _entries)) return;
            _entries = value;
            OnPropertyChanged();
        }
    }
    
    private bool? isAllSelected = true;

    public bool? IsAllSelected
    {
        get { return isAllSelected; }
        set
        {
            isAllSelected = value;
            OnPropertyChanged();
        }
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
    
    public IRelayCommand CheckCommand { get; private set; }
    public IRelayCommand CheckAllCommand { get; private set; }

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
            
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                Entries.Clear();
                foreach (var e in result.Items) Entries.Add(e);
                
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