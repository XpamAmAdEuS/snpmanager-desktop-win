using System;
using System.Collections.Generic;
using Snp.Models;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
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
        
        private readonly UserService.UserServiceClient _client;

        private readonly Mapper _mapper;

        public GrpcUserRepository(CallInvoker invoker,Mapper mapper)
        {
            _client = new UserService.UserServiceClient(invoker);
            _mapper = mapper;
        }
        

        public async Task<User> GetById(string id)
        {
            var request = new  GetUserRequest
            {
                Id = id
            };

            var response = await _client.GetUserAsync(request);
            
            return _mapper.Map<User>(response.User);;
        }
    }
}
