using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SnpApp.Interfaces;

namespace SnpApp.ViewModels;

public class RelayCommandPageViewModel : SamplePageViewModel
{
    public RelayCommandPageViewModel(IFilesService filesService) 
        : base(filesService)
    {
        IncrementCounterCommand = new RelayCommand(IncrementCounter);
    }

    /// <summary>
    /// Gets the <see cref="ICommand"/> responsible for incrementing <see cref="Counter"/>.
    /// </summary>
    public ICommand IncrementCounterCommand { get; }

    private int counter;

    /// <summary>
    /// Gets the current value of the counter.
    /// </summary>
    public int Counter
    {
        get => counter;
        private set => SetProperty(ref counter, value);
    }

    /// <summary>
    /// Increments <see cref="Counter"/>.
    /// </summary>
    private void IncrementCounter() => Counter++;

}
