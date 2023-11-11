using System.ComponentModel;

namespace Snp.Core.Models;


public class SizeLimitModel : INotifyPropertyChanged
{
    #region Constructor

    public SizeLimitModel() {}

    #endregion

    #region Properties

    public ulong Value { get; set; }

    public string Name { get; set; }

    #endregion

    #region Notify Property Changed

    public event PropertyChangedEventHandler PropertyChanged;

    public void NotifyPropertyChanged(string propName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }

    #endregion
}