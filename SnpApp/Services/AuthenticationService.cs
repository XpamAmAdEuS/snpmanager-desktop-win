using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Snp.V1;

namespace SnpApp.Services
{
    
    public sealed class AuthenticationService
    {
        
        private AuthService.AuthServiceClient _client;

        public AuthenticationService()
        {
            var loggerFactory = LoggerFactory.Create(logging =>
            {
                logging.AddConsole();
                logging.AddDebug();
                logging.SetMinimumLevel(LogLevel.Debug);
            });
            
            var channel = GrpcChannel.ForAddress(
                $"{Constants.GrpcUrl}",
                new GrpcChannelOptions
                {
                    Credentials = ChannelCredentials.Insecure,
                    LoggerFactory = loggerFactory,
                });

            _client = new AuthService.AuthServiceClient(channel);
        }
        
        public static AuthenticationService Default { get; } = new();

        public  string GetToken(string username,string password)
        {
            var request = new LoginRequest
            {
                Username = username,
                Password = password
            };
            var token =   _client.Login(request);
            return token.AccessToken;
        }
            

    }
}
