using Snp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Grpc.Core;
using Snp.V1;
using Customer = Snp.Models.Customer;


namespace Snp.Repository.Grpc
{
    /// <summary>
    /// Contains methods for interacting with the customers backend using GRPC. 
    /// </summary>
    public class GrpcCustomerRepository : ICustomerRepository
    {
        
        private readonly CustomerService.CustomerServiceClient _client;
        private readonly Mapper _mapper;

        public GrpcCustomerRepository(CallInvoker invoker,Mapper mapper)
        {
            _client = new CustomerService.CustomerServiceClient(invoker);
            _mapper = mapper;
        }

        public async Task<IEnumerable<Customer>> GetAsync()
        {
           
            var result =  await _client.SearchCustomerAsync(new  SearchCustomerRequest());
            return result.Data.Select(cus => _mapper.Map<Customer>(cus)).ToList();
        }
        
        public async Task<IEnumerable<Customer>> SearchByTitleAsync(string search)
        {
            var result =  await _client.SearchCustomerAsync(new  SearchCustomerRequest());
            return result.Data.Select(cus => _mapper.Map<Customer>(cus)).ToList();
        }
        
        public async Task<Customer> GetOneById(uint id)
        {
            var result =  await _client.GetCustomerAsync(new  GetCustomerRequest
            {
                Id = id
            });
            
            return _mapper.Map<Customer>(result.Customer);
        }
        
        public async Task<Customer> UpsertAsync(Customer customer)
        {
            var result = await  _client.UpdateCustomerAsync(
                new UpdateCustomerRequest
                {
                    Id = customer.Id,
                    Title = customer.Title,
                    Address = customer.Address,
                    Email = customer.Address,
                    Muted = customer.Muted,
                    Person = customer.Person,
                    Phone = customer.Phone,
                    SizeLimit = customer.SizeLimit
                }
                );

            return _mapper.Map<Customer>(result.Customer);
        }



        public 
            async Task DeleteAsync(uint customerId)
        {
            
        } 
           
    }
}
