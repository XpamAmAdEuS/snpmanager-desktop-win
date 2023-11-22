using System;
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
using SnpApp.Common;
using SnpApp.Helper;
using SnpApp.Navigation;
using SnpApp.Services;
using SnpApp.Services.Interceptors;
using SnpApp.ViewModels;
using LaunchActivatedEventArgs = Microsoft.UI.Xaml.LaunchActivatedEventArgs;


namespace SnpApp
{
    public sealed partial class App
    {
        
        private static Window _startupWindow  = default!;
        
        public static Window StartupWindow => _startupWindow;

        public App()
        {
            InitializeComponent();

        }
        
        public void EnableSound(bool withSpatial = false)
        {
            ElementSoundPlayer.State = ElementSoundPlayerState.On;

            ElementSoundPlayer.SpatialAudioMode = !withSpatial ? ElementSpatialAudioMode.Off : ElementSpatialAudioMode.On;
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
            
            JwtTokenHelper.Initialize();

            _startupWindow = WindowHelper.CreateWindow();
            _startupWindow.ExtendsContentIntoTitleBar = true;
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.BindingFailed += DebugSettings_BindingFailed;
            }
#endif
            
            
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

            var token = JwtTokenHelper.JwtToken;
            
            // CallInvoker invoker = channel
            //     .Intercept(new AuthorizationHeaderInterceptor(token))
            //     .Intercept(new ErrorHandlerInterceptor());
            
            var invoker = channel.Intercept(new AuthorizationHeaderInterceptor(token));
            
            Ioc.Default.ConfigureServices
            (new ServiceCollection()
                .AddSingleton<IMessenger>(WeakReferenceMessenger.Default)
                .AddSingleton(new CustomerService(invoker,mapper))
                .AddSingleton(new SiteService(invoker,mapper))
                .AddSingleton(new UserService(invoker,mapper))
                .AddSingleton(new MusicImportService(invoker,mapper))
                .AddSingleton(new MusicService(invoker,mapper))
                .AddTransient<CustomerListViewModel>()
                .AddTransient<MusicListViewModel>()
                .AddTransient<CustomerViewModel>()
                .AddTransient<MusicUploadViewModel>()
                .AddTransient<MusicImportListViewModel>()
                .AddTransient<SiteViewModel>()
                .BuildServiceProvider()
            );
            
            
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
        
        
        private void EnsureWindow()
        {
            
            var RootFrame = GetRootFrame();

            ThemeHelper.Initialize();

            var targetPageType = typeof(HomePage);
            var targetPageArguments = string.Empty;
            
            var eventargs = AppInstance.GetCurrent().GetActivatedEventArgs();
            if (eventargs != null && eventargs.Kind is ExtendedActivationKind.Protocol && eventargs.Data is ProtocolActivatedEventArgs)
            {
                var protocolArgs = eventargs.Data as ProtocolActivatedEventArgs;
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                var uri = protocolArgs.Uri.LocalPath.Replace("/", "");
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                targetPageArguments = uri;
            }

            var rootPage = StartupWindow.Content as NavigationRootPage;
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            rootPage.Navigate(targetPageType, targetPageArguments);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            if (targetPageType == typeof(HomePage))
            {
                ((NavigationViewItem)((NavigationRootPage)StartupWindow.Content).NavigationView.MenuItems[0]).IsSelected = true;
            }
            
            // Ensure the current window is active
            StartupWindow.Activate();
        }

        public Frame GetRootFrame()
        {
            Frame RootFrame;
            var rootPage = StartupWindow.Content as NavigationRootPage;
            if (rootPage == null)
            {
                rootPage = new NavigationRootPage();
                RootFrame = (Frame)rootPage.FindName("RootFrame");
                if (RootFrame == null)
                {
                    throw new Exception("Root frame not found");
                }
                RootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[0];
                RootFrame.NavigationFailed += OnNavigationFailed;
                StartupWindow.Content = rootPage;
            }
            else
            {
                RootFrame = (Frame)rootPage.FindName("RootFrame");
            }

            return RootFrame;
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
            if (e.GetType() == typeof(TokenChangedEventArgs))
            {
                var jwt = e as TokenChangedEventArgs;
                var jwtToken= jwt?.NewValue;
                ApplicationData.Current.LocalSettings.Values[Constants.StoredJwtTokenKey] = jwtToken;
            }
            
            // _mWindow.ExtendsContentIntoTitleBar = true;
            //
            // shell = _mWindow.Content as Shell ?? new Shell();
            // _mWindow.Content = shell;
            
            EnsureWindow();
            
        }
    }
}
