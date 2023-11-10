using AutoMapper;
using Grpc.Core;

namespace Snp.Core.Repository.Grpc
{
    /// <summary>
    /// Contains methods for interacting with the app backend using GRPC. 
    /// </summary>
    public class GrpcSnpRepository : ISnpRepository
    {
        private  CallInvoker _invoker;
        private  Mapper _mapper;
        
        public GrpcSnpRepository(CallInvoker invoker,Mapper mapper)
        {
            _invoker = invoker;
            _mapper = mapper;
        }
        
        public static GrpcSnpRepository Default { get; } = new(null,null);

        public void addDeps(CallInvoker invoker, Mapper mapper)
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
