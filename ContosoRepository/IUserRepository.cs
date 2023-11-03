using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contoso.Models
{
    /// <summary>
    /// Defines methods for interacting with the users backend.
    /// </summary>
    public interface IUserRepository
    {
        
        /// <summary>
        /// Returns the user with the given id. 
        /// </summary>
        Task<User> GetById(string id);
    }
}