﻿using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SnpApp.Common;
using SnpApp.Services;

namespace SnpApp.UserControls
{
    public sealed partial class LoginUserControl: UserControl
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
                    UserNameTextBox.Text = value.ToString();
                    SaveUserNameCheckBox.IsChecked = true;

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
            localSettings.Values[Constants.StoredAccountIdKey] = UserNameTextBox.Text;
        }

        private void ClearUserName()
        {
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values[Constants.StoredAccountIdKey] = null;
            UserNameTextBox.Text = string.Empty;
        }

        private void UserNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Clear the error if the user name field isn't empty.
            if (!string.IsNullOrEmpty(UserNameTextBox.Text))
            {
                ErrorInfoBar.Message = string.Empty;
                ErrorInfoBar.IsOpen = false;
            }
            
            IsEnabled = IsValid();
        }

        private void PasswordTextBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // Clear the error if the password field isn't empty.
            if (!string.IsNullOrEmpty(PasswordTextBox.Password))
            {
                ErrorInfoBar.Message = string.Empty;
                ErrorInfoBar.IsOpen = false;
            }
            
            IsEnabled = IsValid();
        }

        private void PassportSignInButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorInfoBar.Message = string.Empty;
            if (IsValid())
            {
                
                var accessToken =  AuthenticationService.Default.GetToken(UserNameTextBox.Text,PasswordTextBox.Password);
                if (!string.IsNullOrEmpty(accessToken))
                {
                    UserLoggedIn?.Invoke(this, new TokenChangedEventArgs(accessToken));
                }
            }
        }
        
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            
            ErrorInfoBar.Message = "Goodbye";
            UserAbortedLogIn?.Invoke(this, EventArgs.Empty);
            
            
        }
        
        private void RegisterButtonTextBlock_OnPointerPressed(object sender, RoutedEventArgs e)
        {
            ErrorInfoBar.Message = string.Empty;
            
            
            ErrorInfoBar.Message = "Error happened";
        }
        
        private bool IsValid()
        {
            return !string.IsNullOrEmpty(UserNameTextBox.Text) && !string.IsNullOrEmpty(PasswordTextBox.Password);
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
        
        public event EventHandler? UserLoggedIn;
        
        public event EventHandler? UserAbortedLogIn;
    }
    
    
}
