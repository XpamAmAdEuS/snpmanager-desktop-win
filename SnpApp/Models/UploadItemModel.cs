namespace Snp.App.Models;

public class UploadItemModel
{
    public string Name { get; set; }
    public string Path { get; set; }
    
    public double Progress { get; set; }
    
    public bool ShowPaused { get; set; }
    public bool ShowError { get; set; }
}