using CommunityToolkit.Mvvm.ComponentModel;

namespace SnpApp.Models;


public partial class SizeLimitModel : ObservableObject
{
    #region Properties

    [ObservableProperty] private ulong _value;

    [ObservableProperty] private string? _name;

    #endregion
}