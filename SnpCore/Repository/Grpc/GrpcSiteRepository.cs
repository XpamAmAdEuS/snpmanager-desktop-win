using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using SnpCore.Models;
using Snp.V1;



namespace SnpCore.Repository.Grpc
{
   
    public class GrpcSiteRepository
    {
        
        private readonly SiteCrud.SiteCrudClient _client;
        private readonly Mapper _mapper;

        public GrpcSiteRepository(CallInvoker invoker,Mapper mapper)
        {
            _client = new SiteCrud.SiteCrudClient(invoker);
            _mapper = mapper;
        }


        public async Task<IEnumerable<Site>> GetByCustomerId(uint id)
        {
            IEnumerable <Site> result = new List<Site>();
            try
            {
                var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                using var streamingCall = _client.GetListSite(new UInt64Value{Value = id}, cancellationToken: cancellationToken.Token);
                await foreach (var s in streamingCall.ResponseStream.ReadAllAsync(cancellationToken: cancellationToken.Token))
                {
                    result.Append(_mapper.Map<Site>(s));
                    Console.WriteLine(s);
                }
                Console.WriteLine("Stream completed.");
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
            {
                Console.WriteLine("Stream cancelled.");
            }
         
            
            return result;
        }

        public Task<Site> GetOneById(uint id)
        {
            throw new System.NotImplementedException();
        }

        public Task<Site> UpsertAsync(Site site)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteAsync(uint id)
        {
            throw new System.NotImplementedException();
        }
    }
}
