using System;
using Contoso.Models;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Snp.V1;
using User = Contoso.Models.User;


namespace Contoso.Repository.Grpc
{
    /// <summary>
    /// Contains methods for interacting with the customers backend using GRPC. 
    /// </summary>
    public class GrpcUserRepository : IUserRepository
    {
        
        private readonly UserService.UserServiceClient _client;

        public GrpcUserRepository(CallInvoker invoker)
        {
            _client = new UserService.UserServiceClient(invoker);
        }
        

        public async Task<User> GetById(string id)
        {
            var request = new  GetUserRequest();
            var cts = new CancellationTokenSource(); 
            cts.CancelAfter(TimeSpan.FromSeconds(10)); 
            var reply = await _client.GetUserAsync(request, cancellationToken: cts.Token);

            var user = new User().FromPb(reply.User);
            
            return Task.FromResult(user).Result;
        }
    }
}
