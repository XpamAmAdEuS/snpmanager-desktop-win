using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Grpc.Core;
using Snp.Core.Models;
using Snp.V1;
using CrudClient = Snp.V1.ImportMusicCrud.ImportMusicCrudClient;

namespace Snp.Core.Services
{
   
    public class MusicImportService
    {
        
        private readonly CrudClient _client;
        private readonly Mapper _mapper;

        public MusicImportService(CallInvoker invoker,Mapper mapper)
        {
            _client = new CrudClient(invoker);
            _mapper = mapper;
        }


        public async Task<PaginatedList<MusicImport>> SearchImportMusicAsync(SearchRequestModel searchRequestModel)
        {
            try
            {
                var result = await _client.SearchImportMusicAsync(_mapper.Map<SearchRequest>(searchRequestModel));
                var items = result.Data.Select(entity => _mapper.Map<MusicImport>(entity)).ToList();
                var count = (int)result.TotalRecords;
                var pageSize = (int)searchRequestModel.PerPage;
                var pageIndex = (int)searchRequestModel.CurrentPage;
                return new PaginatedList<MusicImport>(items, count, pageIndex, pageSize);
            }
            catch (Exception e)
            {
                throw new RpcException(Status.DefaultCancelled, e.Message);
            }
        }

        public Task<MusicImport> GetOneById(uint id)
        {
            throw new NotImplementedException();
        }

        public Task<MusicImport> UpsertAsync(MusicImport site)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(uint id)
        {
            throw new NotImplementedException();
        }
    }
}
