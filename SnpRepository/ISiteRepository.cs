using System.Collections.Generic;
using System.Threading.Tasks;

namespace Snp.Models
{
    /// <summary>
    /// Defines methods for interacting with the sites backend.
    /// </summary>
    public interface ISiteRepository
    {
        /// <summary>
        /// Returns all customer sites. 
        /// </summary>
        Task<IEnumerable<Site>> GetByCustomerId(uint customerId);

        /// <summary>
        /// Returns the site with the given id. 
        /// </summary>
        Task<Site> GetOneById(uint id);

        /// <summary>
        /// Adds a new site if the site does not exist, updates the 
        /// existing site otherwise.
        /// </summary>
        Task<Site> UpsertAsync(Site site);

        /// <summary>
        /// Deletes a site.
        /// </summary>
        Task DeleteAsync(uint id);
    }
}