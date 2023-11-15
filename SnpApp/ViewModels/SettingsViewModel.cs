using SnpApp.Services;
using System.ComponentModel;

namespace SnpApp.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public bool ToastOnAppEvents
        {
            get
            {
                return SettingsService.Instance.ToastOnAppEvents;
            }
            set
            {
                if (SettingsService.Instance.ToastOnAppEvents != value)
                {
                    SettingsService.Instance.ToastOnAppEvents = value;
                    RaisePropertyChanged("ToastOnAppEvents");
                }
            }
        }

        public bool UseCustomControls
        {
            get
            {
                return SettingsService.Instance.UseCustomControls;
            }
            set
            {
                if (SettingsService.Instance.UseCustomControls != value)
                {
                    SettingsService.Instance.UseCustomControls = value;
                    RaisePropertyChanged("UseCustomControls");
                }
            }
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
