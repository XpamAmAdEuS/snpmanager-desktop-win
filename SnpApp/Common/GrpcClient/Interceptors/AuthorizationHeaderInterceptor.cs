using System;
using Snp.Repository;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;

namespace Snp.App.Common.GrpcClient.Interceptors
{
    public class AuthorizationHeaderInterceptor : Interceptor
    {
        private readonly ILogger<AuthorizationHeaderInterceptor> _logger;
        private const string AuthorizationHeader = "Authorization";
        private readonly string _apiToken;

        public AuthorizationHeaderInterceptor(
            ILogger<AuthorizationHeaderInterceptor> logger,
            Windows.Storage.ApplicationDataContainer configuration
        )
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));
            _logger = logger;
            var apiToken = configuration.Values[Constants.StoredJwtTokenKey]?.ToString();
            _apiToken = apiToken;
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
            TRequest request,
            ClientInterceptorContext<TRequest, TResponse> context,
            AsyncUnaryCallContinuation<TRequest, TResponse> continuation
        )
        {
            
            // Check if the headers are not null, if so, initialize new headers
            if (context.Options.Headers == null)
            {
                _logger.LogDebug("Adding gRPC option headers");
            }
            
            var headers = new Metadata { new (AuthorizationHeader, _apiToken) };

            var newOptions = context.Options.WithHeaders(headers);

            var newContext = new ClientInterceptorContext<TRequest, TResponse>(
                context.Method,
                context.Host,
                newOptions);
 
            return base.AsyncUnaryCall(request, newContext, continuation);
        }
    }
}
