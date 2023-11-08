using Snp.Models;
using System.Threading.Tasks;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Snp.V1;
using User = Snp.Models.User;


namespace Snp.Repository.Grpc
{
    /// <summary>
    /// Contains methods for interacting with the customers backend using GRPC. 
    /// </summary>
    public class GrpcUserRepository : IUserRepository
    {
        
        private readonly UserCrud.UserCrudClient _client;

        private readonly Mapper _mapper;

        public GrpcUserRepository(CallInvoker invoker,Mapper mapper)
        {
            _client = new UserCrud.UserCrudClient(invoker);
            _mapper = mapper;
        }
        

        public async Task<User> GetById(string id)
        {
            var response = await _client.GetOneUserAsync(new StringValue{Value = id});
            return _mapper.Map<User>(response);;
        }
    }
}
