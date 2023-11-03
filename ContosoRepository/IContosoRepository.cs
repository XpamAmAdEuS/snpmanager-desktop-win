using Contoso.Models;

namespace Contoso.Repository
{
    /// <summary>
    /// Defines methods for interacting with the app backend.
    /// </summary>
    public interface IContosoRepository
    {
        /// <summary>
        /// Returns the customers repository.
        /// </summary>
        ICustomerRepository Customers { get; }
        
        ISiteRepository Sites { get; }
        
        IUserRepository Users  { get; }
    }
}
