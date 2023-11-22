using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Grpc.Core;
using SnpApp.Models;
using Snp.V1;


namespace SnpApp.Services
{
    /// <summary>
    /// Contains methods for interacting with the musics backend using GRPC. 
    /// </summary>
    public class MusicService(CallInvoker invoker,Mapper mapper)
    {
        
        private readonly MusicCrud.MusicCrudClient _client = new (invoker);

        public async Task<PaginatedList<Music>> SearchMusicAsync(SearchRequestModel searchRequestModel)
        {
            try
            {
                var result = await _client.SearchMusicAsync(mapper.Map<SearchRequest>(searchRequestModel));
                var items = result.Data.Select(cus => mapper.Map<Music>(cus)).ToList();
                var count = (int)result.TotalRecords;
                var pageSize = (int)searchRequestModel.PerPage;
                var pageIndex = (int)searchRequestModel.CurrentPage;
                return new PaginatedList<Music>(items, count, pageIndex, pageSize);
            }
            catch (Exception e)
            {
                throw new RpcException(Status.DefaultCancelled, e.Message);
            }
        }
    }
}
