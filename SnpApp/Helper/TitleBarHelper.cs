using Microsoft.UI.Xaml;

namespace Snp.App.Helper
{

    internal class TitleBarHelper
    {

        public static void SetCaptionButtonColors(Window window, Windows.UI.Color color)
        {
            var res = Application.Current.Resources;
            res["WindowCaptionForeground"] = color;
            window.AppWindow.TitleBar.ButtonForegroundColor = color;
        }
    }
}
