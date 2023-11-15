using System.ComponentModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using SnpApp.Models;


namespace SnpApp.ViewModels
{
    /// <summary>
    /// Provides a bindable wrapper for the Customer model class, encapsulating various services for access by the UI.
    /// </summary>
    public class MusicImportViewModel : ObservableObject, IEditableObject
    {

        /// <summary>
        /// Initializes a new instance of the MusicImportViewModel class that wraps a MusicImport object.
        /// </summary>
        public MusicImportViewModel(MusicImport? model = null) => Model = model ?? new MusicImport();

        private MusicImport? _model;
        public MusicImport Model
        {
            get => _model;
            set
            {
                if (_model != value)
                {
                    _model = value;
                    OnPropertyChanged(string.Empty);
                }
            }
        }
        
        public string FileName {
            get => Model.FileName;
            set
            {
                if (value != Model.FileName)
                {
                    Model.FileName = value;
                    IsModified = true;
                    OnPropertyChanged();
                }
            }
        }
        public ulong FileSize {
            get => Model.FileSize;
            set
            {
                if (value != Model.FileSize)
                {
                    Model.FileSize = value;
                    IsModified = true;
                    OnPropertyChanged();
                }
            }
        }
        public string Hash {
            get => Model.Hash;
            set
            {
                if (value != Model.Hash)
                {
                    Model.Hash = value;
                    IsModified = true;
                    OnPropertyChanged();
                }
            }
        }
        public ulong Duration {
            get => Model.Duration;
            set
            {
                if (value != Model.Duration)
                {
                    Model.Duration = value;
                    IsModified = true;
                    OnPropertyChanged();
                }
            }
        }
        public string Artist {
            get => Model.Artist;
            set
            {
                if (value != Model.Artist)
                {
                    Model.Artist = value;
                    IsModified = true;
                    OnPropertyChanged();
                }
            }
        }
        public string Title {
            get => Model.Title;
            set
            {
                if (value != Model.Title)
                {
                    Model.Title = value;
                    IsModified = true;
                    OnPropertyChanged();
                }
            }
        }
        public string Album {
            get => Model.Album;
            set
            {
                if (value != Model.Album)
                {
                    Model.Album = value;
                    IsModified = true;
                    OnPropertyChanged();
                }
            }
        }
        public string Genre {
            get => Model.Genre;
            set
            {
                if (value != Model.Genre)
                {
                    Model.Genre = value;
                    IsModified = true;
                    OnPropertyChanged();
                }
            }
        }
        public bool Selected {
            get => Model.Selected;
            set
            {
                if (value != Model.Selected)
                {
                    Model.Selected = value;
                    IsModified = true;
                    OnPropertyChanged();
                }
            }
        }
        
        public bool IsModified { get; set; }
        
        private bool _isLoading;
        
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private bool _isInEdit;

        /// <summary>
        /// Gets or sets a value that indicates whether the customer data is being edited.
        /// </summary>
        public bool IsInEdit
        {
            get => _isInEdit;
            set => SetProperty(ref _isInEdit, value);
        }

        /// <summary>
        /// Saves data that has been edited.
        /// </summary>
        public async Task SaveAsync()
        {
            IsInEdit = false;
            IsModified = false;
            //await Ioc.Default.GetService<>().UpsertAsync(Model);
        }
        
        public async Task CancelEditsAsync()
        {
            await RevertChangesAsync();
            
        }

        /// <summary>
        /// Discards any edits that have been made, restoring the original values.
        /// </summary>
        public async Task RevertChangesAsync()
        {
            IsInEdit = false;
            if (IsModified)
            {
                await RefreshAsync();
                IsModified = false;
            }
        }

        /// <summary>
        /// Enables edit mode.
        /// </summary>
        public void StartEdit() => IsInEdit = true;

        /// <summary>
        /// Reloads all of the customer data.
        /// </summary>
        public async Task RefreshAsync()
        {
            //Model = await Ioc.Default.GetService<MusicImportService>().GetOneById(Model.Id);
        }
        
        /// <summary>
        /// Called when a bound DataGrid control causes the customer to enter edit mode.
        /// </summary>
        public void BeginEdit()
        {
            // Not used.
        }

        /// <summary>
        /// Called when a bound DataGrid control cancels the edits that have been made to a customer.
        /// </summary>
        public async void CancelEdit() => await CancelEditsAsync();

        /// <summary>
        /// Called when a bound DataGrid control commits the edits that have been made to a customer.
        /// </summary>
        public async void EndEdit() => await SaveAsync();
    }
}