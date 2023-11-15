namespace SnpCore.Repository
{
    /// <summary>
    /// Defines methods for interacting with the auth backend.
    /// </summary>
    public interface IAuthRepository
    {
        /// <summary>
        /// Returns token. 
        /// </summary>
        string GetToken(string username,string password);
    }
}