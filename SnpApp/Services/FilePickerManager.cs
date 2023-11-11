using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Snp.Core.Services;
using WinRT.Interop;

namespace Snp.App.Services;

public sealed class FilePickerManager : IFilePickManager
{
    
    public static FilePickerManager Default { get; } = new();
    
    public async Task<IReadOnlyList<StorageFile>?> PickMultipleFilesAsync()
    {
        var picker = new FileOpenPicker();
        picker.FileTypeFilter.Add(".mp3");
        nint windowHandle = WindowNative.GetWindowHandle(App.StartupWindow);
        InitializeWithWindow.Initialize(picker, windowHandle);
        return  await picker.PickMultipleFilesAsync();
    }

    public FolderPicker FolderPicker()
    {
        var picker = new FolderPicker();
        nint windowHandle = WindowNative.GetWindowHandle(App.StartupWindow);
        InitializeWithWindow.Initialize(picker, windowHandle);
        return picker;
    }
}
