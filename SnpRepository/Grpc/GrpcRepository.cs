using AutoMapper;
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
        private readonly Mapper _mapper;
        

        public GrpcSnpRepository(CallInvoker invoker,Mapper mapper)
        {
            _invoker = invoker;
            _mapper = mapper;
        }

        public ICustomerRepository Customers => new GrpcCustomerRepository(_invoker,_mapper);
        public ISiteRepository Sites => new GrpcSiteRepository(_invoker,_mapper);

        public IUserRepository Users => new GrpcUserRepository(_invoker,_mapper);
        public IMusicUploadRepository MusicUploads => new GrpcMusicUploadRepository(_invoker);
    }
}
