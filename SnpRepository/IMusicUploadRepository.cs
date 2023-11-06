using System.Threading.Tasks;

namespace Snp.Models
{
    /// <summary>
    /// Defines methods for interacting with the users backend.
    /// </summary>
    public interface IMusicUploadRepository
    {
        
        /// <summary>
        /// Returns the user with the given id. 
        /// </summary>
        Task Upload(string path,string name);
    }
}