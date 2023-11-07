using Snp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Grpc.Core;
using Snp.V1;
using Site = Snp.Models.Site;


namespace Snp.Repository.Grpc
{
    /// <summary>
    /// Contains methods for interacting with the customers backend using GRPC. 
    /// </summary>
    public class GrpcSiteRepository : ISiteRepository
    {
        
        private readonly SiteService.SiteServiceClient _client;
        private readonly Mapper _mapper;

        public GrpcSiteRepository(CallInvoker invoker,Mapper mapper)
        {
            _client = new SiteService.SiteServiceClient(invoker);
            _mapper = mapper;
        }


        public async Task<IEnumerable<Site>> GetByCustomerId(uint customerId)
        {
         
            var response = await _client.GetSitesAsync(new  GetCustomerSitesRequest{Id = customerId});
            var sites = response.Sites.Select(s => _mapper.Map<Site>(s)).ToList();
            return sites;
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
