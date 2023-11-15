using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SnpApp.Common;
using SnpCore.Repository;
using SnpCore.Repository.Grpc;
using muxc = Microsoft.UI.Xaml.Controls;

namespace SnpApp.Views.Widgets
{
    public sealed partial class LoginUserControl
    {
        
        public new bool IsEnabled { get; set; }
        
        public LoginUserControl()
        {
            InitializeComponent();
            Loaded += LoginUserControl_Loaded;
            Unloaded += LoginUserControl_Unloaded;
            
        }

        private void LoginUserControl_Loaded(object sender, RoutedEventArgs e)
        {

            var autoLoginSucceed = TryAutoLogin();

            if (!autoLoginSucceed)
            {
                // If the user name is saved, get it and populate the user name field.
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                if (localSettings.Values.TryGetValue(Constants.StoredAccountIdKey, value: out var value))
                {
                    userNameTextBox.Text = value.ToString();
                    saveUserNameCheckBox.IsChecked = true;

                    // if (autoLoginSucceed)
                    // {
                    //     var user = UserRepository.GetById(localSettings.Values[StoredAccountIdKey].ToString());
                    //
                    //     if (user.Result != null)
                    //     {
                    //         UserLoggedIn?.Invoke(this, EventArgs.Empty);
                    //     }
                    // }
                
                }
            }
        }

        private void LoginUserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            
        }
        
        private void SaveUserName()
        {
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values[Constants.StoredAccountIdKey] = userNameTextBox.Text;
        }

        private void ClearUserName()
        {
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values[Constants.StoredAccountIdKey] = null;
            userNameTextBox.Text = string.Empty;
        }

        private void UserNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Clear the error if the user name field isn't empty.
            if (!string.IsNullOrEmpty(userNameTextBox.Text))
            {
                errorInfoBar.Message = string.Empty;
                errorInfoBar.IsOpen = false;
            }
            
            IsEnabled = IsValid();
        }

        private void PasswordTextBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // Clear the error if the password field isn't empty.
            if (!string.IsNullOrEmpty(passwordTextBox.Password))
            {
                errorInfoBar.Message = string.Empty;
                errorInfoBar.IsOpen = false;
            }
            
            IsEnabled = IsValid();
        }

        private void PassportSignInButton_Click(object sender, RoutedEventArgs e)
        {
            errorInfoBar.Message = string.Empty;
            if (IsValid())
            {
                
                var accessToken =  GrpcAuthRepository.Default?.GetToken(userNameTextBox.Text,passwordTextBox.Password);
                if (!string.IsNullOrEmpty(accessToken))
                {
                    UserLoggedIn?.Invoke(this, new TokenChangedEventArgs(accessToken));
                }
            }
        }
        
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            
            errorInfoBar.Message = "Goodbye";
            UserAbortedLogIn?.Invoke(this, EventArgs.Empty);
            
            
        }
        
        private void RegisterButtonTextBlock_OnPointerPressed(object sender, RoutedEventArgs e)
        {
            errorInfoBar.Message = string.Empty;
            
            
            errorInfoBar.Message = "Error happened";
        }
        
        private bool IsValid()
        {
            return !string.IsNullOrEmpty(userNameTextBox.Text) && !string.IsNullOrEmpty(passwordTextBox.Password);
        }
        
        private bool TryAutoLogin()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (!localSettings.Values.TryGetValue(Constants.StoredJwtTokenKey, out var value)) return false;
            var token = value.ToString();

            if (!string.IsNullOrEmpty(token))
            {
                UserLoggedIn?.Invoke(this, new TokenChangedEventArgs(token));
                return true;
            }

            return false;
        }
        
        public event EventHandler UserLoggedIn;
        
        public event EventHandler UserAbortedLogIn;
    }
    
    
}
