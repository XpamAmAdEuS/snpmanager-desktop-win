using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Snp.Models;

public class SearchRequestModel:  ObservableObject
{

    private string _searchText = "";
    private string _sortColumn = "id";
    private string _sortDirection = "ASC";
    private uint _perPage = 10;
    private uint _currentPage;
    private List<string> _fields = new (){"id"};
    
    public string SearchText {
        get => _searchText;
        set => SetProperty(ref _searchText, value);
    }
    public string SortColumn {
        get => _sortColumn;
        set => SetProperty(ref _sortColumn, value);
    }
    
    public string SortDirection {
        get => _sortDirection;
        set => SetProperty(ref _sortDirection, value);
    }
    public uint PerPage {
        get => _perPage;
        set => SetProperty(ref _perPage, value);
    }
    public uint CurrentPage {
        get => _currentPage;
        set => SetProperty(ref _currentPage, value);
    }
    
    public List<string> Fields {
        get => _fields;
        set => SetProperty(ref _fields, value);
    }
}