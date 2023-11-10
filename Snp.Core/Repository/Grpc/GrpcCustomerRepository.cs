using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Snp.Core.Models;
using Snp.V1;


namespace Snp.Core.Repository.Grpc
{
    /// <summary>
    /// Contains methods for interacting with the customers backend using GRPC. 
    /// </summary>
    public class GrpcCustomerRepository : ICustomerRepository
    {
        
        private readonly CustomerCrud.CustomerCrudClient _client;
        private readonly Mapper _mapper;

        public GrpcCustomerRepository(CallInvoker invoker,Mapper mapper)
        {
            _client = new CustomerCrud.CustomerCrudClient(invoker);
            _mapper = mapper;
        }

        public async Task<PaginatedList<Customer>> SearchCustomerAsync(SearchRequestModel searchRequestModel)
        {
           
            var result =  await _client.SearchCustomerAsync(_mapper.Map<SearchRequest>(searchRequestModel));
            var items = result.Data.Select(cus => _mapper.Map<Customer>(cus)).ToList();
            int count = (int)result.TotalRecords;
            int pageSize = (int)searchRequestModel.PerPage;
            int pageIndex = (int)searchRequestModel.CurrentPage;
            return new PaginatedList<Customer>(items, count, pageIndex, pageSize);
            
            //return result.Data.Select(cus => _mapper.Map<Customer>(cus)).ToList();
        }
        
        public async Task<IEnumerable<Customer>> SearchByTitleAsync(string search)
        {
            var result =  await _client.SearchCustomerAsync(new  SearchRequest());
            return result.Data.Select(cus => _mapper.Map<Customer>(cus)).ToList();
        }
        
        public async Task<Customer> GetOneById(uint id)
        {
            var result =  await _client.GetOneCustomerAsync(new UInt64Value{Value = id});
            
            return _mapper.Map<Customer>(result);
        }
        
        public async Task<Customer> UpsertAsync(Customer customer)
        {
            var result = await  _client.UpdateCustomerAsync(
                new ProtoCustomerRepo.Types.UpdateRequest
                {
                    ID = customer.Id,
                    Customer =
                    {
                        Title = customer.Title,
                        Address = customer.Address,
                        Email = customer.Address,
                        Muted = customer.Muted,
                        Person = customer.Person,
                        Phone = customer.Phone,
                        SizeLimit = customer.SizeLimit
                    }
                }
                );

            return _mapper.Map<Customer>(result);
        }



        public 
            async Task DeleteAsync(uint customerId)
        {
            
        } 
           
    }
}
