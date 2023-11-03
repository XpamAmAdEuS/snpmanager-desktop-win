using Contoso.Models;
using Grpc.Core;

namespace Contoso.Repository.Grpc
{
    /// <summary>
    /// Contains methods for interacting with the app backend using GRPC. 
    /// </summary>
    public class GrpcContosoRepository : IContosoRepository
    {
        private readonly CallInvoker _invoker; 
        

        public GrpcContosoRepository(CallInvoker invoker)
        {
            _invoker = invoker;
        }

        public ICustomerRepository Customers => new GrpcCustomerRepository(_invoker);
        public ISiteRepository Sites => new GrpcSiteRepository(_invoker);

        public IUserRepository Users => new GrpcUserRepository(_invoker);
    }
}
