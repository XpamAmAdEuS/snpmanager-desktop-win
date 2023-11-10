using System;
using Windows.Foundation;
using CommunityToolkit.Mvvm.DependencyInjection;
using System.Runtime.InteropServices;
using CommunityToolkit.Mvvm.Messaging;
using System.Threading;
using System.Threading.Tasks;
using Windows.System.Profile;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Snp.App.Helper;
using Snp.App.Services;
using Snp.App.Views;

namespace Snp.App
{
    /// <summary>
    /// The "chrome" layer of the app that provides top-level navigation with
    /// proper keyboarding navigation.
    /// </summary>
    public sealed partial class Shell : Page, INavigation
    {
        
        
        public Microsoft.UI.Dispatching.DispatcherQueue dispatcherQueue;
        private RootFrameNavigationHelper _navHelper;
        
        /// <summary>
        /// Initializes a new instance of the AppShell, sets the static 'Current' reference,
        /// adds callbacks for Back requests and changes in the SplitView's DisplayMode, and
        /// provide the nav menu list with the data to display.
        /// </summary>
        public Shell()
        {
            // Title = App.Title;
            //
            InitializeComponent();
            
            dispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();
            
            _navHelper = new RootFrameNavigationHelper(rootFrame, NavigationViewControl);
            
            
            SetDeviceFamily();
            
            
            //
            // var appWindow = this.GetAppWindow();
            // appWindow.SetIcon("Assets/Beer.ico");

            //Root.RequestedTheme = Application.Current.RequestedTheme == ApplicationTheme.Light ? ElementTheme.Light : ElementTheme.Dark;
        }
        
        public Frame AppFrame => rootFrame;
        
        public Action NavigationViewLoaded { get; set; }
        
        public DeviceType DeviceFamily { get; set; }
        
        public string AppTitleText
        {
            get
            {
#if DEBUG
                return "Snp Manager Dev";
#else
                return "Snp Manager";
#endif
            }
        }
        
        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            //Root.RequestedTheme = Root.RequestedTheme == ElementTheme.Light ? ElementTheme.Dark : ElementTheme.Light;

            //Ioc.Default.GetService<IMessenger>().Send(new ThemeChangedMessage(Root.ActualTheme));
        }
        
        private void GoBackInvokerButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (this.rootFrame.CanGoBack)
            {
                this.rootFrame.GoBack();
            }
        }
        
        private static string _error = string.Empty;
        private static string _log = string.Empty;
        
        private async void WaitForIdleInvokerButton_Click(object sender, RoutedEventArgs e)
        {
            _idleStateEnteredCheckBox.IsChecked = false;
            await Windows.System.Threading.ThreadPool.RunAsync(WaitForIdleWorker);

            _logReportingTextBox.Text = _log;

            if (_error.Length == 0)
            {
                _idleStateEnteredCheckBox.IsChecked = true;
            }
            else
            {
                // Setting Text will raise a property-changed event, so even if we
                // immediately set it back to the empty string, we'll still get the
                // error-reported event that we can detect and handle.
                _errorReportingTextBox.Text = _error;
                _errorReportingTextBox.Text = string.Empty;

                _error = string.Empty;
            }
        }
        
        private void CloseAppInvokerButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            Application.Current.Exit();
        }
        
        private static void WaitForIdleWorker(IAsyncAction action)
        {
            _error = IdleSynchronizer.TryWait(out _log);
        }
        
        private void WaitForDebuggerInvokerButton_Click(object sender, RoutedEventArgs e)
        {
            DebuggerAttachedCheckBox.IsChecked = false;

            var dispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();

            var workItem = new Windows.System.Threading.WorkItemHandler((IAsyncAction _) =>
            {
                while (!IsDebuggerPresent())
                {
                    Thread.Sleep(1000);
                }

                DebugBreak();

                dispatcherQueue.TryEnqueue(
                    Microsoft.UI.Dispatching.DispatcherQueuePriority.Low,
                    new Microsoft.UI.Dispatching.DispatcherQueueHandler(() =>
                    {
                        DebuggerAttachedCheckBox.IsChecked = true;
                    }));
            });

            var asyncAction = Windows.System.Threading.ThreadPool.RunAsync(workItem);
        }
        
        [DllImport("kernel32.dll")]
        private static extern bool IsDebuggerPresent();

        [DllImport("kernel32.dll")]
        private static extern void DebugBreak();
        
        
        private void OnPaneDisplayModeChanged(NavigationView sender, NavigationViewDisplayModeChangedEventArgs args)
        {
            if (sender.PaneDisplayMode == NavigationViewPaneDisplayMode.Top)
            {
                VisualStateManager.GoToState(this, "Top", true);
            }
            else
            {
                if (args.DisplayMode == NavigationViewDisplayMode.Minimal)
                {
                    VisualStateManager.GoToState(this, "Compact", true);
                }
                else
                {
                    VisualStateManager.GoToState(this, "Default", true);
                }
            }
        }
        
        private void OnNavigationViewControlLoaded(object sender, RoutedEventArgs e)
        {
            // Delay necessary to ensure NavigationView visual state can match navigation
            Task.Delay(500).ContinueWith(_ => this.NavigationViewLoaded?.Invoke(), TaskScheduler.FromCurrentSynchronizationContext());

            var navigationView = sender as NavigationView;
            navigationView.RegisterPropertyChangedCallback(NavigationView.IsPaneOpenProperty, OnIsPaneOpenChanged);
            //TODO check win ui reason
            NavigationView_Loaded(sender, e);
            
        }
        
        private void OnIsPaneOpenChanged(DependencyObject sender, DependencyProperty dp)
        {
            var navigationView = sender as NavigationView;
            var announcementText = navigationView.IsPaneOpen ? "Navigation Pane Opened" : "Navigation Pane Closed";

            UIHelper.AnnounceActionForAccessibility(navigationView, announcementText, "NavigationViewPaneIsOpenChangeNotificationId");
            
        }
        
        public void Navigate(
            Type pageType,
            object targetPageArguments = null,
            Microsoft.UI.Xaml.Media.Animation.NavigationTransitionInfo navigationTransitionInfo = null)
        {
            NavigationRootPageArgs args = new NavigationRootPageArgs();
            args.NavigationRootPage = this;
            args.Parameter = targetPageArguments;
            rootFrame.Navigate(pageType, args, navigationTransitionInfo);
        }
        
        
        
        private void OnNavigatingToPage(object sender, NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {
                // if (e.SourcePageType == typeof(CustomerListPage))
                // {
                //     NavigationView.SelectedItem = CustomerListMenuItem;
                // }
                // else if (e.SourcePageType == typeof(OrderListPage))
                // {
                //     NavigationView.SelectedItem = OrderListMenuItem;
                // }
                // else if (e.SourcePageType == typeof(SettingsPage))
                // {
                //     NavigationView.SelectedItem = NavigationView.SettingsItem;
                // }
            }
        }
        
        private void OnRootFrameNavigated(object sender, NavigationEventArgs e)
        {
            TestContentLoadedCheckBox.IsChecked = true;
        }

        private void OnRootFrameNavigating(object sender, NavigatingCancelEventArgs e)
        {
            TestContentLoadedCheckBox.IsChecked = false;
        }
        
        public Microsoft.UI.Xaml.Controls.NavigationView NavigationView
        {
            get { return NavigationViewControl; }
        }
        
        public class NavigationRootPageArgs
        {
            public Shell NavigationRootPage;
            public object Parameter;
        }
        
        public enum DeviceType
        {
            Desktop,
            Mobile,
            Other,
            Xbox
        }
        
        private void SetDeviceFamily()
        {
            var familyName = AnalyticsInfo.VersionInfo.DeviceFamily;

            if (!Enum.TryParse(familyName.Replace("Windows.", string.Empty), out DeviceType parsedDeviceType))
            {
                parsedDeviceType = DeviceType.Other;
            }

            DeviceFamily = parsedDeviceType;
        }
        
    }
}
