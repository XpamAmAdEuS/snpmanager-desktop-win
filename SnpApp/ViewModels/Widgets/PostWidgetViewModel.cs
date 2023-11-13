// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Snp.Core.Models;

namespace Snp.App.ViewModels.Widgets;

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
