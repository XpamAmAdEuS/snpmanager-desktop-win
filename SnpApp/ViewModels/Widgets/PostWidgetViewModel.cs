using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using SnpApp.Models;

namespace SnpApp.ViewModels.Widgets;

/// <summary>
/// A viewmodel for a post widget.
/// </summary>
public sealed class PostWidgetViewModel : ObservableRecipient, IRecipient<PropertyChangedMessage<UploadItem>>
{
    private UploadItem? post;

    /// <summary>
    /// Gets the currently selected post, if any.
    /// </summary>
    public UploadItem? Post
    {
        get => post;
        private set => SetProperty(ref post, value);
    }

    /// <inheritdoc/>
    public void Receive(PropertyChangedMessage<UploadItem> message)
    {
        if (message.Sender.GetType() == typeof(SubredditWidgetViewModel) &&
            message.PropertyName == nameof(SubredditWidgetViewModel.SelectedPost))
        {
            Post = message.NewValue;
        }
    }
}
