using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Snp.V1;

namespace Snp.Core.Repository.Grpc
{
    
    public sealed class GrpcAuthRepository : IAuthRepository
    {
        
        private AuthService.AuthServiceClient _client;

        public GrpcAuthRepository()
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
        
        public static GrpcAuthRepository Default { get; } = new();

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
