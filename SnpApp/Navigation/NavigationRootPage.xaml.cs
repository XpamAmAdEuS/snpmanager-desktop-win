using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.System.Profile;
using Windows.UI.Notifications;
using Windows.UI.ViewManagement;
using Microsoft.UI.Xaml.Automation.Peers;
using SnpApp.Helper;
using SnpApp.Services;
using SnpApp.Views;
using WindowActivatedEventArgs = Microsoft.UI.Xaml.WindowActivatedEventArgs;

namespace SnpApp.Navigation
{
    public sealed partial class NavigationRootPage : Page
    {
        public Windows.System.VirtualKey ArrowKey;
        public Microsoft.UI.Dispatching.DispatcherQueue dispatcherQueue;
        private RootFrameNavigationHelper _navHelper;
        private UISettings _settings;
        public static NavigationRootPage Current;


        public static NavigationRootPage GetForElement(object obj)
        {
            UIElement element = (UIElement)obj;
            Window window = WindowHelper.GetWindowForElement(element);
            if (window != null)
            {
                return (NavigationRootPage)window.Content;
            }
            return null;
        }

        public Microsoft.UI.Xaml.Controls.NavigationView NavigationView
        {
            get { return NavigationViewControl; }
        }

        public Action NavigationViewLoaded { get; set; }

        public DeviceType DeviceFamily { get; set; }

        public string AppTitleText
        {
            get
            {
#if DEBUG
                return "SnpManager Dev";
#else
                return "SnpManager";
#endif
            }
        }

        public NavigationRootPage()
        {
            this.InitializeComponent();
            dispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();

            _navHelper = new RootFrameNavigationHelper(rootFrame, NavigationViewControl);

            SetDeviceFamily();
            
            GotFocus += (object sender, RoutedEventArgs e) =>
            {
                // helpful for debugging focus problems w/ keyboard & gamepad
                if (FocusManager.GetFocusedElement() is FrameworkElement focus)
                {
                    Debug.WriteLine("got focus: " + focus.Name + " (" + focus.GetType().ToString() + ")");
                }
            };

            // remove the solid-colored backgrounds behind the caption controls and system back button if we are in left mode
            // This is done when the app is loaded since before that the actual theme that is used is not "determined" yet
            Loaded += delegate (object sender, RoutedEventArgs e)
            {
                NavigationOrientationHelper.UpdateNavigationViewForElement(NavigationOrientationHelper.IsLeftMode(), this);

                Window window = WindowHelper.GetWindowForElement(sender as UIElement);
                window.Title = AppTitleText;
                window.ExtendsContentIntoTitleBar = true;
                window.Activated += Window_Activated;
                window.SetTitleBar(this.AppTitleBar);

                AppWindow appWindow = WindowHelper.GetAppWindow(window);
                appWindow.SetIcon("Assets/Tiles/GalleryIcon.ico");
                _settings = new UISettings();
                _settings.ColorValuesChanged += _settings_ColorValuesChanged; // cannot use FrameworkElement.ActualThemeChanged event because the triggerTitleBarRepaint workaround no longer works
            };
            
            
            Current = this;
        }

        private void Window_Activated(object sender, WindowActivatedEventArgs args)
        {
            if (args.WindowActivationState == WindowActivationState.Deactivated)
            {
                VisualStateManager.GoToState(this, "Deactivated", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "Activated", true);
            }
        }

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

        // this handles updating the caption button colors correctly when indows system theme is changed
        // while the app is open
        private void _settings_ColorValuesChanged(UISettings sender, object args)
        {
            // This calls comes off-thread, hence we will need to dispatch it to current app's thread
            dispatcherQueue.TryEnqueue(() =>
            {
                _ = TitleBarHelper.ApplySystemThemeToCaptionButtons(App.StartupWindow);
            });
        }

        // Wraps a call to rootFrame.Navigate to give the Page a way to know which NavigationRootPage is navigating.
        // Please call this function rather than rootFrame.Navigate to navigate the rootFrame.
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

        public void EnsureNavigationSelection(string id)
        {
            foreach (object rawGroup in this.NavigationView.MenuItems)
            {
                if (rawGroup is NavigationViewItem group)
                {
                    foreach (object rawItem in group.MenuItems)
                    {
                        if (rawItem is NavigationViewItem item)
                        {
                            if ((string)item.Tag == id)
                            {
                                group.IsExpanded = true;
                                NavigationView.SelectedItem = item;
                                item.IsSelected = true;
                                return;
                            }
                            else if (item.MenuItems.Count > 0)
                            {
                                foreach (var rawInnerItem in item.MenuItems)
                                {
                                    if (rawInnerItem is NavigationViewItem innerItem)
                                    {
                                        if ((string)innerItem.Tag == id)
                                        {
                                            group.IsExpanded = true;
                                            item.IsExpanded = true;
                                            NavigationView.SelectedItem = innerItem;
                                            innerItem.IsSelected = true;
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
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

        private void OnNavigationViewControlLoaded(object sender, RoutedEventArgs e)
        {
            // Delay necessary to ensure NavigationView visual state can match navigation
            Task.Delay(500).ContinueWith(_ => this.NavigationViewLoaded?.Invoke(), TaskScheduler.FromCurrentSynchronizationContext());

            var navigationView = sender as NavigationView;
            navigationView.RegisterPropertyChangedCallback(NavigationView.IsPaneOpenProperty, OnIsPaneOpenChanged);
        }

        private void OnIsPaneOpenChanged(DependencyObject sender, DependencyProperty dp)
        {
            var navigationView = sender as NavigationView;
            var announcementText = navigationView.IsPaneOpen ? "Navigation Pane Opened" : "Navigation Pane Closed";

            UIHelper.AnnounceActionForAccessibility(navigationView, announcementText, "NavigationViewPaneIsOpenChangeNotificationId");
        }

        private void OnNavigationViewSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                if (rootFrame.CurrentSourcePageType != typeof(SettingsPage))
                {
                    Navigate(typeof(SettingsPage));
                }
            }
            else
            {
                var selectedItem = args.SelectedItemContainer;
                if (selectedItem == Home)
                {
                    if (rootFrame.CurrentSourcePageType != typeof(HomePage))
                    {
                        Navigate(typeof(HomePage));
                    }
                }
                else if (selectedItem == MusicUpload)
                {
                    if (rootFrame.CurrentSourcePageType != typeof(MusicUploadPage))
                    {
                        Navigate(typeof(MusicUploadPage));
                    }
                }
                else if (selectedItem == MusicImport)
                {
                    if (rootFrame.CurrentSourcePageType != typeof(MusicImportPage))
                    {
                        Navigate(typeof(MusicImportPage));
                    }
                }
                else if (selectedItem == WaveForm)
                {
                    if (rootFrame.CurrentSourcePageType != typeof(WaveformPage))
                    {
                        Navigate(typeof(WaveformPage));
                    }
                }
                
                else if (selectedItem == Test)
                {
                    if (rootFrame.CurrentSourcePageType != typeof(TestPage))
                    {
                        Navigate(typeof(TestPage));
                    }
                }
                else if (selectedItem == Customer2)
                {
                    if (rootFrame.CurrentSourcePageType != typeof(CustomerListPage))
                    {
                        Navigate(typeof(CustomerListPage));
                    }
                }
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

        public bool EnsureItemIsVisibleInNavigation(string name)
        {
            bool changedSelection = false;
            foreach (object rawItem in NavigationView.MenuItems)
            {
                // Check if we encountered the separator
                if (!(rawItem is NavigationViewItem))
                {
                    // Skipping this item
                    continue;
                }

                var item = rawItem as NavigationViewItem;

                // Check if we are this category
                if ((string)item.Content == name)
                {
                    NavigationView.SelectedItem = item;
                    changedSelection = true;
                }
                // We are not :/
                else
                {
                    // Maybe one of our items is?
                    if (item.MenuItems.Count != 0)
                    {
                        foreach (NavigationViewItem child in item.MenuItems)
                        {
                            if ((string)child.Content == name)
                            {
                                // We are the item corresponding to the selected one, update selection!

                                // Deal with differences in displaymodes
                                if (NavigationView.PaneDisplayMode == NavigationViewPaneDisplayMode.Top)
                                {
                                    // In Topmode, the child is not visible, so set parent as selected
                                    // Everything else does not work unfortunately
                                    NavigationView.SelectedItem = item;
                                    item.StartBringIntoView();
                                }
                                else
                                {
                                    // Expand so we animate
                                    item.IsExpanded = true;
                                    // Ensure parent is expanded so we actually show the selection indicator
                                    NavigationView.UpdateLayout();
                                    // Set selected item
                                    NavigationView.SelectedItem = child;
                                    child.StartBringIntoView();
                                }
                                // Set to true to also skip out of outer for loop
                                changedSelection = true;
                                // Break out of child iteration for loop
                                break;
                            }
                        }
                    }
                }
                // We updated selection, break here!
                if (changedSelection)
                {
                    break;
                }
            }
            return changedSelection;
        }

        #region Helpers for test automation

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

        private static void WaitForIdleWorker(IAsyncAction action)
        {
            _error = IdleSynchronizer.TryWait(out _log);
        }

        private void CloseAppInvokerButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            Application.Current.Exit();
        }

        private void GoBackInvokerButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (this.rootFrame.CanGoBack)
            {
                this.rootFrame.GoBack();
            }
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

        #endregion
        
        
         /// <summary>
        /// Display a message to the user.
        /// This method may be called from any thread.
        /// </summary>
        /// <param name="strMessage"></param>
        /// <param name="type"></param>
        public void NotifyUser(string strMessage, NotifyType type)
        {
            // If called from the UI thread, then update immediately.
            // Otherwise, schedule a task on the UI thread to perform the update.

            dispatcherQueue.TryEnqueue(() =>
            {
                UpdateStatus(strMessage, type);
            });
        }

        private void UpdateStatus(string strMessage, NotifyType type)
        {
            switch (type)
            {
                case NotifyType.StatusMessage:
                    StatusBorder.Background = new SolidColorBrush(Microsoft.UI.Colors.Green);
                    break;
                case NotifyType.ErrorMessage:
                    StatusBorder.Background = new SolidColorBrush(Microsoft.UI.Colors.Red);
                    break;
            }

            StatusBlock.Text = strMessage;

            // Collapse the StatusBlock if it has no text to conserve real estate.
            StatusBorder.Visibility = (StatusBlock.Text != String.Empty) ? Visibility.Visible : Visibility.Collapsed;
            if (StatusBlock.Text != String.Empty)
            {
                StatusBorder.Visibility = Visibility.Visible;
                StatusPanel.Visibility = Visibility.Visible;
            }
            else
            {
                StatusBorder.Visibility = Visibility.Collapsed;
                StatusPanel.Visibility = Visibility.Collapsed;
            }

			// Raise an event if necessary to enable a screen reader to announce the status update.
			var peer = FrameworkElementAutomationPeer.FromElement(StatusBlock);
			if (peer != null)
			{
				peer.RaiseAutomationEvent(AutomationEvents.LiveRegionChanged);
			}
		}

        /// <summary>
        /// Sends a toast notification
        /// </summary>
        /// <param name="msg">Message to send</param>
        /// <param name="subMsg">Sub message</param>
        public void ShowToast(string msg, string subMsg = null)
        {
            
            if (!SettingsService.Instance.ToastOnAppEvents)
                return;

            var toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);

            var toastTextElements = toastXml.GetElementsByTagName("text");
            toastTextElements[0].AppendChild(toastXml.CreateTextNode(msg));
            toastTextElements[1].AppendChild(toastXml.CreateTextNode(subMsg));

            var toast = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

    }
    
    
    

    public class NavigationRootPageArgs
    {
        public NavigationRootPage NavigationRootPage;
        public object Parameter;
    }

    public enum DeviceType
    {
        Desktop,
        Mobile,
        Other,
        Xbox
    }
    
    public enum NotifyType
    {
        StatusMessage,
        ErrorMessage
    };
}
