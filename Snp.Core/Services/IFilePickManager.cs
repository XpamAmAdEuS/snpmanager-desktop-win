#nullable enable
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Snp.Core.Services;

public interface IFilePickManager
{
    
    Task<IReadOnlyList<StorageFile>?> PickMultipleFilesAsync();
    
    FolderPicker? FolderPicker();
}