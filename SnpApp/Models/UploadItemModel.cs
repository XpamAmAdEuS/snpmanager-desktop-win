using CommunityToolkit.Mvvm.ComponentModel;

namespace Snp.App.Models;

public class UploadItemModel :  ObservableObject
{
    
    
    private string _name;
    private string _path;
    private double _progress;
    private bool _showPaused;
    private bool _showError;
    
    public string Name{
        get => _name;
        set => SetProperty(ref _name, value);
    }
    public string Path{
        get => _path;
        set => SetProperty(ref _path, value);
    }
    
    public double Progress{
        get => _progress;
        set => SetProperty(ref _progress, value);
    }
    
    public bool ShowPaused{
        get => _showPaused;
        set => SetProperty(ref _showPaused, value);
    }
    public bool ShowError{
        get => _showError;
        set => SetProperty(ref _showError, value);
    }
}