using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.UI.Xaml;

namespace Snp.App.Services
{
    public class ThemeChangedMessage : ValueChangedMessage<ElementTheme>
    {
        public ThemeChangedMessage(ElementTheme value) : base(value)
        {
        }
    }
}
