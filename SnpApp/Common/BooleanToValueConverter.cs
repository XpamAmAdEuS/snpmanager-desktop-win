using System;
using Microsoft.UI.Xaml.Data;

namespace SnpApp.Common
{
    public sealed class BooleanToValueConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return ((bool)value) ? parameter : null;
#pragma warning restore CS8603 // Possible null reference return.
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
