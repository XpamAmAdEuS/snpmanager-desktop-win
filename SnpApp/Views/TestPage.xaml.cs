using System;
using System.Threading.Tasks;
using Windows.Media.Audio;
using Windows.Media.Render;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using SnpApp.Helper;
using WinRT.Interop;


namespace SnpApp.Views
{
    /// <summary>
    /// Using the AudioGraph API to playback from a file input.
    /// </summary>
    public sealed partial class TestPage
    {

        private AudioGraph? _graph;
        private AudioFileInputNode? _fileInput;
        private AudioDeviceOutputNode? _deviceOutput;

        public TestPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await CreateAudioGraph();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Destroy the graph if the page is naviated away from
            if (_graph != null)
            {
                _graph.Dispose();
            }
        }

        private async void File_Click(object sender, RoutedEventArgs e)
        {

            // If another file is already loaded into the FileInput node
            if (_fileInput != null)
            {
                // Release the file and dispose the contents of the node
                _fileInput.Dispose();
                // Stop playback since a new file is being loaded. Also reset the button UI
                if (graphButton.Content.Equals("Stop Graph"))
                {
                    TogglePlay();
                }
            }

            var filePicker = new FileOpenPicker();
            filePicker.SuggestedStartLocation = PickerLocationId.MusicLibrary;
            filePicker.FileTypeFilter.Add(".mp3");
            filePicker.FileTypeFilter.Add(".wav");
            filePicker.FileTypeFilter.Add(".wma");
            filePicker.FileTypeFilter.Add(".m4a");
            filePicker.ViewMode = PickerViewMode.Thumbnail;
            
            // Retrieve the window handle (HWND) of the current WinUI 3 window.
            var window = WindowHelper.GetWindowForElement(this);
            var hWnd = WindowNative.GetWindowHandle(window);

            // Initialize the folder picker with the window handle (HWND).
            InitializeWithWindow.Initialize(filePicker, hWnd);
            
            
            StorageFile file = await filePicker.PickSingleFileAsync();

            // File can be null if cancel is hit in the file picker
            if(file == null)
            {
                return;
            }

            CreateAudioFileInputNodeResult fileInputResult = await _graph.CreateFileInputNodeAsync(file);
            if (AudioFileNodeCreationStatus.Success != fileInputResult.Status)
            {
                // Cannot read input file
                return;
            }

            _fileInput = fileInputResult.FileInputNode;

            if (_fileInput.Duration <= TimeSpan.FromSeconds(3))
            {
                // Imported file is too short
                _fileInput.Dispose();
                _fileInput = null;
                return;
            }

            _fileInput.AddOutgoingConnection(_deviceOutput);
            fileButton.Background = new SolidColorBrush(Colors.Green);

            // Trim the file: set the start time to 3 seconds from the beginning
            // fileInput.EndTime can be used to trim from the end of file
            _fileInput.StartTime = TimeSpan.FromSeconds(3);

            // Enable buttons in UI to start graph, loop and change playback speed factor
            graphButton.IsEnabled = true;
            loopToggle.IsEnabled = true;
            playSpeedSlider.IsEnabled = true;
        }

        private void Graph_Click(object sender, RoutedEventArgs e)
        {
            TogglePlay();
        }

        private void TogglePlay()
        { 
            //Toggle playback
            if (graphButton.Content.Equals("Start Graph"))
            {
                _graph.Start();
                graphButton.Content = "Stop Graph";
                audioPipe.Fill = new SolidColorBrush(Colors.Blue);
            }
            else
            {
                _graph.Stop();
                graphButton.Content = "Start Graph";
                audioPipe.Fill = new SolidColorBrush(Color.FromArgb(255, 49, 49, 49));
            }
        }

        private void PlaySpeedSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (_fileInput != null)
            {
                _fileInput.PlaybackSpeedFactor = playSpeedSlider.Value;
            }
        }

        private void LoopToggle_Toggled(object sender, RoutedEventArgs e)
        {
            // Set loop count to null for infinite looping
            // Set loop count to 0 to stop looping after current iteration
            // Set loop count to non-zero value for finite looping
            if (loopToggle.IsOn)
            {
                // If turning on looping, make sure the file hasn't finished playback yet
                if (_fileInput.Position >= _fileInput.Duration)
                {
                    // If finished playback, seek back to the start time we set
                    _fileInput.Seek(_fileInput.StartTime.Value);
                }
                _fileInput.LoopCount = null; // infinite looping
            }
            else
            {
                _fileInput.LoopCount = 0; // stop looping
            }
        }

        private async Task CreateAudioGraph()
        {
            // Create an AudioGraph with default settings
            AudioGraphSettings settings = new AudioGraphSettings(AudioRenderCategory.Media);
            CreateAudioGraphResult result = await AudioGraph.CreateAsync(settings);

            if (result.Status != AudioGraphCreationStatus.Success)
            {
                // Cannot create graph
                return;
            }

            _graph = result.Graph;

            // Create a device output node
            CreateAudioDeviceOutputNodeResult deviceOutputNodeResult = await _graph.CreateDeviceOutputNodeAsync();

            if (deviceOutputNodeResult.Status != AudioDeviceNodeCreationStatus.Success)
            {
                // Cannot create device output node
                speakerContainer.Background = new SolidColorBrush(Colors.Red);
                return;
            }

            _deviceOutput = deviceOutputNodeResult.DeviceOutputNode;
            speakerContainer.Background = new SolidColorBrush(Colors.Green);
        }
    }
}