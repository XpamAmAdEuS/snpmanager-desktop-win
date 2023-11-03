﻿using System;
using System.Reflection;
using Microsoft.UI.Xaml;
using Windows.Globalization;
using Windows.Storage;
using Contoso.App.Common;
using Contoso.App.Common.GrpcClient.Interceptors;
using Contoso.App.Helper;
using Contoso.App.UserControls;
using Contoso.App.ViewModels;
using Contoso.Models;
using Contoso.Repository;
using Contoso.Repository.Grpc;

using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;


namespace Contoso.App
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {

        /// <summary>
        /// Gets main App Window
        /// </summary>
        public static Window Window => _mWindow;

        private static Window _mWindow;

        /// <summary>
        /// Gets the app-wide MainViewModel singleton instance.
        /// </summary>
        public static MainViewModel ViewModel { get; set; }
        
        const string StoredAccountIdKey = "accountid";
        const string StoredAuthorityKey = "authority";

        /// <summary>
        /// Pipeline for interacting with backend service or database.
        /// </summary>
        public static IContosoRepository Repository { get; private set; }
        
        public static readonly IAuthRepository AuthRepository = new GrpcAuthRepository($"{Constants.GrpcUrl}");
        
        private static readonly ILoggerFactory LogFactory = LoggerFactory.Create(logging =>
        {
            logging.AddConsole();
            logging.AddDebug();
            logging.SetMinimumLevel(LogLevel.Debug);
        });

        private ILogger<App> logger = LogFactory.CreateLogger<App>();
            
        private static readonly ChannelBase Channel = GrpcChannel.ForAddress(
            Constants.GrpcUrl,
            new GrpcChannelOptions
            {
                Credentials = ChannelCredentials.Insecure,
                LoggerFactory = LogFactory
            });

        private static CallInvoker _invoker;
        
        // private static readonly CallInvoker Invoker = Channel
        //     .Intercept(new AuthorizationHeaderInterceptor(LogFactory.CreateLogger<AuthorizationHeaderInterceptor>(),ApplicationData.Current.LocalSettings))
        //     .Intercept(new ErrorHandlerInterceptor())
        //     .Intercept(new ClientLoggerInterceptor(LogFactory));

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App() => InitializeComponent();
        

        public static TEnum GetEnum<TEnum>(string text) where TEnum : struct
        {
            if (!typeof(TEnum).GetTypeInfo().IsEnum)
            {
                throw new InvalidOperationException("Generic parameter 'TEnum' must be an enum.");
            }
            return (TEnum)Enum.Parse(typeof(TEnum), text);
        }
        

        /// <summary>
        /// Invoked when the application is launched normally by the end user.
        /// </summary>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs e)
        {
            //m_window = new MainWindow();
            
            _mWindow = WindowHelper.CreateWindow();
            _mWindow.ExtendsContentIntoTitleBar = true;
            
            // Create LoginUserControl and attach event handlers
            LoginUserControl loginUserControl = new LoginUserControl();
            loginUserControl.UserLoggedIn += LoginUserControl_UserLoggedIn;
            loginUserControl.UserAbortedLogIn += LoginUserControl_UserAbortedLogIn;
            loginUserControl.CreateRepository += LoginUserControl_CreateRepository;
            
            
            _mWindow.Content = loginUserControl;
            
            ThemeHelper.Initialize();
            
            // Ensure the current window is active
            _mWindow.Activate();
        }
        
        
        public static bool UseGrpc()
        {
            
            try
            {
                
                _invoker = Channel
                    .Intercept(new AuthorizationHeaderInterceptor(LogFactory.CreateLogger<AuthorizationHeaderInterceptor>(),ApplicationData.Current.LocalSettings))
                    .Intercept(new ErrorHandlerInterceptor());
                
                Repository = new GrpcContosoRepository(_invoker);
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }
        
        private bool HasAccountStored()
        {
            return ApplicationData.Current.LocalSettings.Values.ContainsKey(StoredAccountIdKey);
        }
        
        private bool HasTokenStored()
        {
            var keyExist=  ApplicationData.Current.LocalSettings.Values.ContainsKey(StoredAccountIdKey);

            if (!keyExist) return false;
            if (!ApplicationData.Current.LocalSettings.Values.TryGetValue(
                    Constants.SelectedAppJwtTokenKey, out object token)) return false;
            return token != null;
        }
        
        // private bool HasAccountStored()
        // {
        //     return (ApplicationData.Current.LocalSettings.Values.ContainsKey(StoredAccountIdKey) &&
        //             ApplicationData.Current.LocalSettings.Values.ContainsKey(StoredAuthorityKey));
        // }
        
        private void RemoveAccountData()
        {
            ApplicationData.Current.LocalSettings.Values.Remove(StoredAccountIdKey);
            ApplicationData.Current.LocalSettings.Values.Remove(StoredAuthorityKey);
        }
        
        /// <summary>
        /// Login was aborted.
        /// </summary>
        private void LoginUserControl_UserAbortedLogIn(object sender, EventArgs e)
        {
            RemoveAccountData();
            _mWindow.Close();
        }
        
        private void LoginUserControl_CreateRepository(object sender, EventArgs e)
        {
            UseGrpc();

        }

        /// <summary>
        /// Login was successful.
        /// </summary>
        private void LoginUserControl_UserLoggedIn(object sender, EventArgs e)
        {

            if (e.GetType() == typeof(TokenChangedEventArgs))
            {
                var test = e as TokenChangedEventArgs;
                ApplicationData.Current.LocalSettings.Values[Constants.SelectedAppJwtTokenKey] = test.NewValue;
                UseGrpc();
            }
            // EventAggregator.GetEvent<UserLoginSuccessful>().Publish(e.LoginSuccessfulResult);
            
            // Prepare the app shell and window content.
            AppShell shell = _mWindow.Content as AppShell ?? new AppShell();
            shell.Language = ApplicationLanguages.Languages[0];
            _mWindow.Content = shell;

            //UseGrpc();
            
            ViewModel = new();
            
            ThemeHelper.Initialize();
            
            // Ensure the current window is active
            _mWindow.Activate();
        }
    }
}
