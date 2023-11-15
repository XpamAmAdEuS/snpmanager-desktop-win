using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.UI.Xaml;

namespace SnpApp.Services
{
    public class ThemeChangedMessage : ValueChangedMessage<ElementTheme>
    {
        public ThemeChangedMessage(ElementTheme value) : base(value)
        {
        }
    }
}
