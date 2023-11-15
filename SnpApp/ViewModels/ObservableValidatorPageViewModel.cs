using SnpApp.Interfaces;

namespace SnpApp.ViewModels;

public partial class ObservableValidatorPageViewModel : SamplePageViewModel
{
    public ObservableValidatorPageViewModel(IFilesService filesService)
        : base(filesService)
    {
    }
}
