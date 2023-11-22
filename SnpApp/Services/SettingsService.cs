using System;
using Windows.Foundation.Collections;
using Windows.Storage;

namespace SnpApp.Services
{
    class SettingsService
    {
        static SettingsService? _instance;

        public static SettingsService Instance => _instance ??= new SettingsService();

        private const string ToastOnAppEventsKey = "toast-on-app-events";
        private const string UseCustomControlsKey = "use-custom-controls";
        private readonly IPropertySet _settings = ApplicationData.Current.RoamingSettings.Values;

        public event EventHandler? UseCustomControlsChanged;

        public bool ToastOnAppEvents
        {
            get
            {
                if (_settings.TryGetValue(ToastOnAppEventsKey, out var setting))
                    return (bool)setting;

                return true;
            }
            set => _settings[ToastOnAppEventsKey] = value;
        }

        public bool UseCustomControls
        {
            get
            {
                if (_settings.TryGetValue(UseCustomControlsKey, out var setting))
                    return (bool)setting;

                return false;
            }
            set
            {
                if (UseCustomControls == value) return;
                _settings[UseCustomControlsKey] = value;
                UseCustomControlsChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
