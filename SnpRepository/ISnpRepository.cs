using Snp.Models;

namespace Snp.Repository
{
    /// <summary>
    /// Defines methods for interacting with the app backend.
    /// </summary>
    public interface ISnpRepository
    {
        /// <summary>
        /// Returns the customers repository.
        /// </summary>
        ICustomerRepository Customers { get; }
        
        ISiteRepository Sites { get; }
        
        IUserRepository Users  { get; }
    }
}
