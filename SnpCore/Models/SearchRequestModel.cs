using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI.UI.Controls;

namespace SnpCore.Models;

public partial class SearchRequestModel:  ObservableObject
{
    
    [ObservableProperty]
    private string searchText = "";
    
    [ObservableProperty]
    private string sortColumn = "id";
    
    [ObservableProperty]
    private DataGridSortDirection sortDirection = DataGridSortDirection.Ascending;
    
    [ObservableProperty]
    private uint perPage = 10;

    [ObservableProperty]
    private uint currentPage;
    
    [ObservableProperty]
    private List<string> fields = new (){"id"};
}