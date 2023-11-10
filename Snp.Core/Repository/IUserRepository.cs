using System.Threading.Tasks;
using Snp.Core.Models;

namespace Snp.Core.Repository
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