using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;

namespace SnpApp.Helper
{
    public static class UiHelper
    {
        
        // Confirmation of Action
        public static void AnnounceActionForAccessibility(UIElement ue, string announcement, string activityId)
        {
            var peer = FrameworkElementAutomationPeer.FromElement(ue);
            peer.RaiseNotificationEvent(AutomationNotificationKind.ActionCompleted,
                                        AutomationNotificationProcessing.ImportantMostRecent, announcement, activityId);
        }
    }
}
