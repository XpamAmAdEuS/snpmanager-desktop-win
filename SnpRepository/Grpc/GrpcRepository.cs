using Snp.Models;
using Grpc.Core;

namespace Snp.Repository.Grpc
{
    /// <summary>
    /// Contains methods for interacting with the app backend using GRPC. 
    /// </summary>
    public class GrpcSnpRepository : ISnpRepository
    {
        private readonly CallInvoker _invoker; 
        

        public GrpcSnpRepository(CallInvoker invoker)
        {
            _invoker = invoker;
        }

        public ICustomerRepository Customers => new GrpcCustomerRepository(_invoker);
        public ISiteRepository Sites => new GrpcSiteRepository(_invoker);

        public IUserRepository Users => new GrpcUserRepository(_invoker);
        public IMusicUploadRepository MusicUploads => new GrpcMusicUploadRepository(_invoker);
    }
}
