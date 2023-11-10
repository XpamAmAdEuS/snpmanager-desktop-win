using System;
using Windows.Storage;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Snp.App.Common;
using Snp.App.Helper;
using Snp.App.Views.Widgets;
using Snp.Core.ViewModels;
using Snp.Core.Repository;
using Snp.Core.Repository.Grpc;
using Snp.Core.Repository.Grpc.Interceptors;


namespace Snp.App
{
    public sealed partial class App : Application
    {
        
        /// <summary>
        /// Gets main App Window
        /// </summary>
        public static Window Window => _mWindow;

        private static Window _mWindow;
        
        private Shell shell;
        
        public static string Title => "Snp Manager";

        public App()
        {
            InitializeComponent();
        }
        
        public new static App Current => (App)Application.Current;
        
        
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            
            // Create LoginUserControl and attach event handlers
            LoginUserControl loginUserControl = new LoginUserControl();
            loginUserControl.UserLoggedIn += LoginUserControl_UserLoggedIn;
            loginUserControl.UserAbortedLogIn += LoginUserControl_UserAbortedLogIn;


            _mWindow = WindowHelper.CreateWindow();

            _mWindow.Content = loginUserControl;
            _mWindow.Activate();
        }
        
        
        
        /// <summary>
        /// Login was aborted.
        /// </summary>
        private void LoginUserControl_UserAbortedLogIn(object sender, EventArgs e)
        {
            // RemoveAccountData();
            // _mWindow.Close();
        }
        
        /// <summary>
        /// Login was successful.
        /// </summary>
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
                    .AddSingleton<ISnpRepository>(new GrpcSnpRepository(invoker,mapper))
                    .AddSingleton<IConnection>(Connection.Default)
                    .AddTransient<CustomerListViewModel>()
                    .AddTransient<CustomerViewModel>()
                    .AddTransient<MusicUploadViewModel>()
                    .AddTransient<SiteViewModel>()
                    .BuildServiceProvider()
                );
            }
            
            shell = _mWindow.Content as Shell ?? new Shell();
            _mWindow.Content = shell;
            
            _mWindow.Activate();
        }
    }
}
