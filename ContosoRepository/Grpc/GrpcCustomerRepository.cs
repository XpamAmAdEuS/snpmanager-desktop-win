using Contoso.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Snp.V1;
using Customer = Contoso.Models.Customer;


namespace Contoso.Repository.Grpc
{
    /// <summary>
    /// Contains methods for interacting with the customers backend using GRPC. 
    /// </summary>
    public class GrpcCustomerRepository : ICustomerRepository
    {
        
        private readonly CustomerService.CustomerServiceClient _client;

        public GrpcCustomerRepository(CallInvoker invoker)
        {
            
            _client = new CustomerService.CustomerServiceClient(invoker);
        }

        public async Task<IEnumerable<Customer>> GetAsync()
        {
            var request = new  SearchCustomerRequest();
            var result =  await _client.SearchCustomerAsync(request);

            var customers = new List<Customer>();

            foreach (var cus in result.Data)
            {
                var c = new Customer();
                c.FromPb(cus);
                customers.Add(c);
            }
            
            return Task.FromResult(customers).GetAwaiter().GetResult();
        }
        
        public async Task<IEnumerable<Customer>> SearchByTitleAsync(string search)
        {
            var request = new  SearchCustomerRequest();
            var result =  await _client.SearchCustomerAsync(request);

            var customers = new List<Customer>();

            foreach (var cus in result.Data)
            {
                var c = new Models.Customer();
                c.FromPb(result.Data[0]);
                customers.Add(c);
            }
            
            return Task.FromResult(customers).GetAwaiter().GetResult();
        }
        
        public async Task<Customer> GetOneById(uint id)
        {
            var request = new  SearchCustomerRequest();
            var result =  await _client.SearchCustomerAsync(request);
            

            var c = new Customer();
            c.FromPb(result.Data[0]);
            
            return Task.FromResult(c).GetAwaiter().GetResult();
        }
        
        public async Task<Customer> UpsertAsync(Customer customer)
        {
            var request = new  SearchCustomerRequest();
            var result = await  _client.SearchCustomerAsync(request);

            var c = new Models.Customer();
            c.FromPb(result.Data[0]);
            
            return Task.FromResult(c).GetAwaiter().GetResult();
        }



        public async Task DeleteAsync(uint customerId)
        {
            
        } 
           
    }
}
