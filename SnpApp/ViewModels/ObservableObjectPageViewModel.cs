using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SnpApp.Interfaces;

namespace SnpApp.ViewModels;

public class ObservableObjectPageViewModel : SamplePageViewModel
{
    public ObservableObjectPageViewModel(IFilesService filesService) 
        : base(filesService)
    {
        ReloadTaskCommand = new RelayCommand(ReloadTask);
    }

    /// <summary>
    /// Gets the <see cref="ICommand"/> responsible for setting <see cref="MyTask"/>.
    /// </summary>
    public ICommand ReloadTaskCommand { get; }

    private string? name;

    /// <summary>
    /// Gets or sets the name to display.
    /// </summary>
    public string? Name
    {
        get => name;
        set => SetProperty(ref name, value);
    }

    private TaskNotifier? myTask;

    /// <summary>
    /// Gets or sets the name to display.
    /// </summary>
    public Task? MyTask
    {
        get => myTask;
        private set => SetPropertyAndNotifyOnCompletion(ref myTask, value);
    }

    /// <summary>
    /// Simulates an asynchronous method.
    /// </summary>
    public void ReloadTask()
    {
        MyTask = Task.Delay(3000);
    }
}
