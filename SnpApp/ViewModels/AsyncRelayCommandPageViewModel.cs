using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using SnpApp.Interfaces;


namespace SnpApp.ViewModels;

public class AsyncRelayCommandPageViewModel : SamplePageViewModel
{
    public AsyncRelayCommandPageViewModel(IFilesService filesService) 
        : base(filesService)
    {
        DownloadTextCommand = new AsyncRelayCommand(DownloadTextAsync);
    }

    public IAsyncRelayCommand DownloadTextCommand { get; }

    private async Task<string> DownloadTextAsync()
    {
        await Task.Delay(3000); // Simulate a web request

        return "Hello world!";
    }
}
