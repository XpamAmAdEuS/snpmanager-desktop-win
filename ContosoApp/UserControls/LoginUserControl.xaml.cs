//  ---------------------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// 
//  The MIT License (MIT)
// 
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
// 
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
// 
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//  ---------------------------------------------------------------------------------

using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.Foundation;
using Contoso.App.Common;
using Contoso.Models;
using Contoso.Repository;
using muxc = Microsoft.UI.Xaml.Controls;

namespace Contoso.App.UserControls
{
    
    public enum SignInResult
    {
        SignInOK,
        SignInFail,
        SignInCancel,
        Nothing
    }
    public sealed partial class LoginUserControl : UserControl
    {
        
        const string StoredAccountIdKey = "accountid";
        
        private static IAuthRepository AuthRepository => App.AuthRepository;
        
        private static IUserRepository UserRepository => App.Repository.Users;
        
        public new bool IsEnabled { get; set; }
        
        public SignInResult Result { get; private set; }
        
        public LoginUserControl()
        {
            InitializeComponent();
            Loaded += LoginUserControl_Loaded;
            Unloaded += LoginUserControl_Unloaded;
            
        }

        // Using a DependencyProperty as the backing store for CollapseWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CollapseWidthProperty =
            DependencyProperty.Register("CollapseWidth", typeof(double), typeof(LoginUserControl), new PropertyMetadata(0.0));
        

        private void LoginUserControl_Loaded(object sender, RoutedEventArgs e)
        {

            var autoLoginSucceed = TryAutoLogin();

            if (!autoLoginSucceed)
            {
                // If the user name is saved, get it and populate the user name field.
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                if (localSettings.Values.TryGetValue(StoredAccountIdKey, value: out var value))
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

                IsEnabled = IsValid();
            }
            Result = SignInResult.Nothing;

           

        }

        private void LoginUserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            
        }
        
        private void SaveUserName()
        {
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values[StoredAccountIdKey] = userNameTextBox.Text;
        }

        private void ClearUserName()
        {
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values[StoredAccountIdKey] = null;
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
                
                var accessToken =  AuthRepository.GetToken(userNameTextBox.Text,passwordTextBox.Password);
                if (!string.IsNullOrEmpty(accessToken))
                {
                    
                    UserLoggedIn?.Invoke(this, new TokenChangedEventArgs(accessToken));
                    //ApplicationData.Current.LocalSettings.Values[Constants.SelectedAppJwtTokenKey] = accessToken;
                }
                
            }
            
            //UserLoggedIn?.Invoke(this, EventArgs.Empty);
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
            if (!localSettings.Values.TryGetValue(Constants.SelectedAppJwtTokenKey, out var value)) return false;
            var token = value.ToString();

            if (!string.IsNullOrEmpty(token))
            {
                UserLoggedIn?.Invoke(this, EventArgs.Empty);
                return true;
            }

            return false;
        }
        
        public event EventHandler UserLoggedIn;
        
        public event EventHandler UserAbortedLogIn;
        
        public event EventHandler CreateRepository;
    }
    
    
}
