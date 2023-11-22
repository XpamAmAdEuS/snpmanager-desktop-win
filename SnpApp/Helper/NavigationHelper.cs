using Windows.System;
using Microsoft.UI.Xaml.Controls;
using Windows.UI.Core;
using Microsoft.UI.Input;

namespace SnpApp.Helper
{
    
    /// <summary>
    /// RootFrameNavigationHelper registers for standard mouse and keyboard
    /// shortcuts used to go back and forward. There should be only one
    /// RootFrameNavigationHelper per view, and it should be associated with the
    /// root frame.
    /// </summary>
    /// <example>
    /// To make use of RootFrameNavigationHelper, create an instance of the
    /// RootNavigationHelper such as in the constructor of your root page.
    /// <code>
    ///     public MyRootPage()
    ///     {
    ///         this.InitializeComponent();
    ///         this.rootNavigationHelper = new RootNavigationHelper(MyFrame);
    ///     }
    /// </code>
    /// </example>
    [Windows.Foundation.Metadata.WebHostHidden]
    public class RootFrameNavigationHelper
    {
        private Frame Frame { get; set; }
        private NavigationView CurrentNavView { get; set; }
        private bool isKeyDownProcessed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="RootFrameNavigationHelper"/> class.
        /// </summary>
        /// <param name="RootFrame">A reference to the top-level frame.
        /// This reference allows for frame manipulation and to register navigation handlers.</param>
        public RootFrameNavigationHelper(Frame RootFrame, NavigationView currentNavView)
        {
            this.Frame = RootFrame;
            this.Frame.Navigated += (s, e) =>
            {
                // Update the Back button whenever a navigation occurs.
                UpdateBackButton();
            };
            this.CurrentNavView = currentNavView;

            CurrentNavView.BackRequested += NavView_BackRequested;
            CurrentNavView.KeyDown += CurrentNavView_KeyDown;
            CurrentNavView.KeyUp += CurrentNavView_KeyUp;
        }

        private void NavView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            TryGoBack();
        }

        private bool TryGoBack()
        {
            bool navigated = false;
            // Don't go back if the nav pane is overlayed.
            if (this.CurrentNavView.IsPaneOpen && (this.CurrentNavView.DisplayMode == NavigationViewDisplayMode.Compact || this.CurrentNavView.DisplayMode == NavigationViewDisplayMode.Minimal))
            {
                return navigated;
            }

            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
                navigated = true;
            }

            return navigated;
        }

        private bool TryGoForward()
        {
            bool navigated = false;
            if (this.Frame.CanGoForward)
            {
                this.Frame.GoForward();
                navigated = true;
            }
            return navigated;
        }

        private void UpdateBackButton()
        {
            this.CurrentNavView.IsBackEnabled = this.Frame.CanGoBack ? true : false;
        }

        /// <summary>
        /// Invoked on every keystroke, including system keys such as Alt key combinations.
        /// Used to detect keyboard navigation between pages even when the page itself
        /// doesn't have focus.
        /// </summary>
        /// <param name="sender">Instance that triggered the event.</param>
        /// <param name="e">Event data describing the conditions that led to the event.</param>

        private void CurrentNavView_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Handled || isKeyDownProcessed)
            {
                return;
            }

            var virtualKey = e.Key;

            // Only investigate further when Left, Right, or the dedicated
            // Previous or Next keys are pressed.
            if (virtualKey == VirtualKey.Left || virtualKey == VirtualKey.Right ||
                (int)virtualKey == 166 || (int)virtualKey == 167)
            {
                var downState = CoreVirtualKeyStates.Down;
                // VirtualKeys 'Menu' key is also the 'Alt' key on the keyboard.
                bool isMenuKeyPressed = (InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Menu) & downState) == downState;
                bool isControlKeyPressed = (InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Control) & downState) == downState;
                bool isShiftKeyPressed = (InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Shift) & downState) == downState;
                bool isModifierKeyPressed = !isMenuKeyPressed && !isControlKeyPressed && !isShiftKeyPressed;
                bool isOnlyAltPressed = isMenuKeyPressed && !isControlKeyPressed && !isShiftKeyPressed;

                if (((int)virtualKey == 166 && isModifierKeyPressed) ||
                    (virtualKey == VirtualKey.Left && isOnlyAltPressed))
                {
                    // When the previous key or Alt+Left are pressed navigate back.
                    e.Handled = TryGoBack();
                }
                else if (((int)virtualKey == 167 && isModifierKeyPressed) ||
                    (virtualKey == VirtualKey.Right && isOnlyAltPressed))
                {
                    // When the next key or Alt+Right are pressed navigate forward.
                    e.Handled = TryGoForward();
                }
                isKeyDownProcessed = e.Handled;
            }
        }

        private void CurrentNavView_KeyUp(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            isKeyDownProcessed = false;
        }
    }
}
