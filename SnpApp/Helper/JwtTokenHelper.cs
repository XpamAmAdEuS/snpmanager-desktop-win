using Windows.Storage;

namespace SnpApp.Helper
{
    /// <summary>
    /// Class providing functionality around switching and restoring theme settings
    /// </summary>
    public static class JwtTokenHelper
    {
        private const string SelectedAppJwtTokenKey = Constants.StoredJwtTokenKey;

        
        /// <summary>
        /// Gets or sets (with LocalSettings persistence) the RequestedTheme of the root element.
        /// </summary>
        public static string JwtToken
        {
            get
            {
                if (NativeHelper.IsAppPackaged)
                {
                    var savedJwtToken = ApplicationData.Current.LocalSettings.Values[SelectedAppJwtTokenKey]?.ToString();

                    if (savedJwtToken != null)
                    {
                        return savedJwtToken;
                    }
                }

                return "";
            }
            set
            {
                
                if (NativeHelper.IsAppPackaged)
                {
                    ApplicationData.Current.LocalSettings.Values[SelectedAppJwtTokenKey] = value;
                }
            }
        }

        public static void Initialize()
        {
            if (NativeHelper.IsAppPackaged)
            {
                var savedJwtToken = ApplicationData.Current.LocalSettings.Values[SelectedAppJwtTokenKey]?.ToString();

                if (savedJwtToken != null)
                {
                    JwtToken = savedJwtToken;
                }
            }
        }
    }
}
