using SnpApp.Interfaces;

namespace SnpApp.ViewModels;

public class IocPageViewModel : SamplePageViewModel
{
    public IocPageViewModel(IFilesService filesService) 
        : base(filesService)
    {
    }
}
