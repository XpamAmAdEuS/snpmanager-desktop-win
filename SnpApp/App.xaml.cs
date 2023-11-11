﻿using System;
using System.Linq;
using System.Reflection;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.AppLifecycle;
using Snp.App.Common;
using Snp.App.DataModel;
using Snp.App.Helper;
using Snp.App.Navigation;
using Snp.App.Services;
using Snp.App.Views;
using Snp.App.Views.Widgets;
using Snp.Core.ViewModels;
using Snp.Core.Repository;
using Snp.Core.Repository.Grpc;
using Snp.Core.Repository.Grpc.Interceptors;
using Snp.Core.Services;
using LaunchActivatedEventArgs = Microsoft.UI.Xaml.LaunchActivatedEventArgs;


namespace Snp.App
{
    public sealed partial class App : Application
    {
        
        private static Window startupWindow;
        
        public static Window StartupWindow
        {
            get
            {
                return startupWindow;
            }
        }
        
        
        public static string Title => "Snp Manager";

        public App()
        {
            InitializeComponent();
            
#if WINUI_PRERELEASE
            this.Suspending += OnSuspending;
            this.Resuming += App_Resuming;
            this.RequiresPointerMode = ApplicationRequiresPointerMode.WhenRequested;
#endif
        }
        
        public void EnableSound(bool withSpatial = false)
        {
            ElementSoundPlayer.State = ElementSoundPlayerState.On;

            if (!withSpatial)
                ElementSoundPlayer.SpatialAudioMode = ElementSpatialAudioMode.Off;
            else
                ElementSoundPlayer.SpatialAudioMode = ElementSpatialAudioMode.On;
        }
        
        public static TEnum GetEnum<TEnum>(string text) where TEnum : struct
        {
            if (!typeof(TEnum).GetTypeInfo().IsEnum)
            {
                throw new InvalidOperationException("Generic parameter 'TEnum' must be an enum.");
            }
            return (TEnum)Enum.Parse(typeof(TEnum), text);
        }
        
        
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            
            IdleSynchronizer.Init();

            startupWindow = WindowHelper.CreateWindow();
            startupWindow.ExtendsContentIntoTitleBar = true;
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.BindingFailed += DebugSettings_BindingFailed;
            }
#endif
            
            
            EnsureWindow();
            
            
            // Create LoginUserControl and attach event handlers
            // LoginUserControl loginUserControl = new LoginUserControl();
            // loginUserControl.UserLoggedIn += LoginUserControl_UserLoggedIn;
            // loginUserControl.UserAbortedLogIn += LoginUserControl_UserAbortedLogIn;
            
        }
        
        
        private void DebugSettings_BindingFailed(object sender, BindingFailedEventArgs e)
        {

        }

#if WINUI_PRERELEASE
        protected override void OnActivated(IActivatedEventArgs args)
        {
            EnsureWindow(args);
        }
#endif
        
        
        private async void EnsureWindow(IActivatedEventArgs args = null)
        {
            // No matter what our destination is, we're going to need control data loaded - let's knock that out now.
            // We'll never need to do this again.
            await ControlInfoDataSource.Instance.GetGroupsAsync();
            await IconsDataSource.Instance.LoadIcons();

            Frame rootFrame = GetRootFrame();

            ThemeHelper.Initialize();

            Type targetPageType = typeof(HomePage);
            string targetPageArguments = string.Empty;

            if (args != null)
            {
                if (args.Kind == ActivationKind.Launch)
                {
                    if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                    {
                        try
                        {
                            await SuspensionManager.RestoreAsync();
                        }
                        catch (SuspensionManagerException)
                        {
                            //Something went wrong restoring state.
                            //Assume there is no state and continue
                        }
                    }

                    targetPageArguments = ((Windows.ApplicationModel.Activation.LaunchActivatedEventArgs)args).Arguments;
                }
            }
            var eventargs = Microsoft.Windows.AppLifecycle.AppInstance.GetCurrent().GetActivatedEventArgs();
            if (eventargs != null && eventargs.Kind is ExtendedActivationKind.Protocol && eventargs.Data is ProtocolActivatedEventArgs)
            {
                ProtocolActivatedEventArgs ProtocolArgs = eventargs.Data as ProtocolActivatedEventArgs;
                var uri = ProtocolArgs.Uri.LocalPath.Replace("/", "");

                targetPageArguments = uri;
                string targetId = string.Empty;

                if (uri == "AllControls")
                {
                    targetPageType = typeof(AllControlsPage);
                }
                else if (uri == "NewControls")
                {
                    targetPageType = typeof(HomePage);
                }
                else if (ControlInfoDataSource.Instance.Groups.Any(g => g.UniqueId == uri))
                {
                    targetPageType = typeof(SectionPage);
                }
                else if (ControlInfoDataSource.Instance.Groups.Any(g => g.Items.Any(i => i.UniqueId == uri)))
                {
                    targetPageType = typeof(ItemPage);
                }
            }

            NavigationRootPage rootPage = StartupWindow.Content as NavigationRootPage;
            rootPage.Navigate(targetPageType, targetPageArguments);

            if (targetPageType == typeof(HomePage))
            {
                ((Microsoft.UI.Xaml.Controls.NavigationViewItem)((NavigationRootPage)App.StartupWindow.Content).NavigationView.MenuItems[0]).IsSelected = true;
            }

            // Ensure the current window is active
            StartupWindow.Activate();
        }

        public Frame GetRootFrame()
        {
            Frame rootFrame;
            NavigationRootPage rootPage = StartupWindow.Content as NavigationRootPage;
            if (rootPage == null)
            {
                rootPage = new NavigationRootPage();
                rootFrame = (Frame)rootPage.FindName("rootFrame");
                if (rootFrame == null)
                {
                    throw new Exception("Root frame not found");
                }
                SuspensionManager.RegisterFrame(rootFrame, "AppFrame");
                rootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[0];
                rootFrame.NavigationFailed += OnNavigationFailed;

                StartupWindow.Content = rootPage;
            }
            else
            {
                rootFrame = (Frame)rootPage.FindName("rootFrame");
            }

            return rootFrame;
        }
        
        
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }
        
        
        private void LoginUserControl_UserAbortedLogIn(object sender, EventArgs e)
        {
            // RemoveAccountData();
            // _mWindow.Close();
        }
        
        private void LoginUserControl_UserLoggedIn(object sender, EventArgs e)
        {

            var jwtToken= "";
            
            if (e.GetType() == typeof(TokenChangedEventArgs))
            {
                var jwt = e as TokenChangedEventArgs;
                jwtToken = jwt.NewValue;
                ApplicationData.Current.LocalSettings.Values[Constants.StoredJwtTokenKey] = jwtToken;
                
                
                var mapper = MapperConfig.InitializeAutomapper();
            
                ChannelBase channel = GrpcChannel.ForAddress(
                    Constants.GrpcUrl,
                    new GrpcChannelOptions
                    {
                        Credentials = ChannelCredentials.Insecure,
                        LoggerFactory = LoggerFactory.Create(logging =>
                        {
                            logging.AddConsole();
                            logging.AddDebug();
                            logging.SetMinimumLevel(LogLevel.Debug);
                        })
                    });
            
                CallInvoker invoker = channel
                    .Intercept(new AuthorizationHeaderInterceptor(jwtToken))
                    .Intercept(new ErrorHandlerInterceptor());
            
                Ioc.Default.ConfigureServices
                (new ServiceCollection()
                    .AddSingleton<IMessenger>(WeakReferenceMessenger.Default)
                    .AddSingleton<IFilePickManager>(FilePickerManager.Default)
                    .AddSingleton<ISnpRepository>(new GrpcSnpRepository(invoker,mapper))
                    .AddSingleton<IConnection>(Connection.Default)
                    .AddTransient<CustomerListViewModel>()
                    .AddTransient<CustomerViewModel>()
                    .AddTransient<MusicUploadViewModel>()
                    .AddTransient<SiteViewModel>()
                    .BuildServiceProvider()
                );
            }
            
            // _mWindow.ExtendsContentIntoTitleBar = true;
            //
            // shell = _mWindow.Content as Shell ?? new Shell();
            // _mWindow.Content = shell;
            
            EnsureWindow();
            
        }
    }
    
    
   
    
}
