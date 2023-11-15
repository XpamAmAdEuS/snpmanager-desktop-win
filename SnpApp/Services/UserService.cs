using System.Threading.Tasks;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Snp.V1;
using SnpApp.Models;


namespace SnpApp.Services
{
    /// <summary>
    /// Contains methods for interacting with the customers backend using GRPC. 
    /// </summary>
    public class UserService
    {
        
        private readonly UserCrud.UserCrudClient _client;

        private readonly Mapper _mapper;

        public UserService(CallInvoker invoker,Mapper mapper)
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
