using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contoso.Models
{
    /// <summary>
    /// Defines methods for interacting with the customers backend.
    /// </summary>
    public interface ICustomerRepository
    {
        /// <summary>
        /// Returns all customers. 
        /// </summary>
        Task<IEnumerable<Customer>> GetAsync();

        /// <summary>
        /// Returns all customers with a data field matching the start of the given string. 
        /// </summary>
        Task<IEnumerable<Customer>> SearchByTitleAsync(string search);

        /// <summary>
        /// Returns the customer with the given id. 
        /// </summary>
        Task<Customer> GetOneById(uint id);

        /// <summary>
        /// Adds a new customer if the customer does not exist, updates the 
        /// existing customer otherwise.
        /// </summary>
        Task<Customer> UpsertAsync(Customer customer);

        /// <summary>
        /// Deletes a customer.
        /// </summary>
        Task DeleteAsync(uint customerId);
    }
}