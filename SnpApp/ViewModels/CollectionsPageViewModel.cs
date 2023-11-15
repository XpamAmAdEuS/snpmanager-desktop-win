using SnpApp.Interfaces;

namespace SnpApp.ViewModels;

public partial class CollectionsPageViewModel : SamplePageViewModel
{
    public CollectionsPageViewModel(IFilesService filesService) 
        : base(filesService)
    {
    }
}
