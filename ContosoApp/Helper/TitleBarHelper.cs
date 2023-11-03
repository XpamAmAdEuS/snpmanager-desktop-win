using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using WinRT;
using System.Runtime.InteropServices;

namespace Contoso.App.Helper
{

    internal class TitleBarHelper
    {
        // workaround as Appwindow titlebar doesn't update caption button colors correctly when changed while app is running
        // https://task.ms/44172495
        public static Windows.UI.Color ApplySystemThemeToCaptionButtons(Window window)
        {
           // var frame = (Application.Current as App).GetRootFrame() as FrameworkElement;
            Windows.UI.Color color = Colors.White;
            // if (frame.ActualTheme == ElementTheme.Dark)
            // {
            //     color = Colors.White;
            // }
            // else
            // {
            //     color = Colors.Black;
            // }
            // SetCaptionButtonColors(window,color);
            return color;
        }

        public static void SetCaptionButtonColors(Window window, Windows.UI.Color color)
        {
            var res = Application.Current.Resources;
            res["WindowCaptionForeground"] = color;
            window.AppWindow.TitleBar.ButtonForegroundColor = color;
        }
    }
}
