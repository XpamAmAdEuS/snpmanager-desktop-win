using System;
using Contoso.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Snp.V1;
using Site = Contoso.Models.Site;


namespace Contoso.Repository.Grpc
{
    /// <summary>
    /// Contains methods for interacting with the customers backend using GRPC. 
    /// </summary>
    public class GrpcSiteRepository : ISiteRepository
    {
        
        private readonly SiteService.SiteServiceClient _client;

        public GrpcSiteRepository(CallInvoker invoker)
        {
            _client = new SiteService.SiteServiceClient(invoker);
        }


        public async Task<IEnumerable<Site>> GetByCustomerId(uint customerId)
        {
            var request = new  GetCustomerSitesRequest();
            var cts = new CancellationTokenSource(); 
            cts.CancelAfter(TimeSpan.FromSeconds(10)); 
            var reply = await _client.GetSitesAsync(request, cancellationToken: cts.Token);

            var sites = new List<Site>();

            foreach (var s in reply.Sites)
            {
                var site = new Site();
                site.FromPb(s);
                sites.Add(site);
            }
            
            
            return Task.FromResult(sites).GetAwaiter().GetResult();
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
