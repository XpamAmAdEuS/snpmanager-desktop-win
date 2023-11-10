using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;

namespace Snp.Core.Repository.Grpc;

public class Connection :IConnection
{
    private ChannelBase? _channelBase;

    public ChannelBase ChannelBase
    {
        get => _channelBase;
    }
    
    
    public ILoggerFactory LogFactory = LoggerFactory.Create(logging =>
    {
        logging.AddConsole();
        logging.AddDebug();
        logging.SetMinimumLevel(LogLevel.Debug);
    });
    
    public Connection()
    {
        ChannelBase channel = GrpcChannel.ForAddress(
            Constants.GrpcUrl,
            new GrpcChannelOptions
            {
                Credentials = ChannelCredentials.Insecure,
                LoggerFactory = LogFactory
            });
        
        
        _channelBase =  channel;
    }
    
    public static Connection Default { get; } = new();
    
}